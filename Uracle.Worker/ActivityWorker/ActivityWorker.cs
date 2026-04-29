using MediatR;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Application.Commands.StravaCommand;
using Uracle.Infrastructure.Services;
using static Uracle.Application.Abstractions.Interfaces.IRedisQueueService;

namespace Uracle.Worker.ActivityWorker;

/// <summary>
/// Background service that dequeues Strava webhook events from Redis
/// and processes them via CQRS commands.
/// </summary>
public class ActivityWorker : BackgroundService
{
    private const int MaxAttempts = 3;

    private readonly IRedisQueueService _redisQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ActivityWorker> _logger;

    public ActivityWorker(
        IRedisQueueService redisQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<ActivityWorker> logger)
    {
        _redisQueue = redisQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ActivityWorker started");

        await _redisQueue.ConnectAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _redisQueue.PopFromQueueAsync(
                    RedisQueueService.ActivityQueue, timeoutSeconds: 5, stoppingToken);

                if (message == null) continue;

                await ProcessMessageAsync(message, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ActivityWorker main loop");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("ActivityWorker stopped");
    }

    private async Task ProcessMessageAsync(QueueMessage message, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

        await eventRepository.UpdateEventStatusAsync(message.Id, "Processing", cancellationToken: ct);
        await eventRepository.IncrementAttemptsAsync(message.Id, ct);

        try
        {
            var result = await DispatchCommandAsync(sender, message, ct);

            if (result)
            {
                await eventRepository.UpdateEventStatusAsync(message.Id, "Successful", cancellationToken: ct);
                _logger.LogInformation("Event {EventId} processed successfully", message.Id);
            }
            else
            {
                await HandleFailureAsync(message, "Processing returned false", ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event {EventId}", message.Id);
            await HandleFailureAsync(message, ex.Message, ct, ex);
        }
    }

    private async Task<bool> DispatchCommandAsync(ISender sender, QueueMessage message, CancellationToken ct)
    {
        var evt = message.Data.Event;

        if (evt.Object_Type == "activity")
        {
            var result = evt.Aspect_Type switch
            {
                "create" => await sender.Send(
                    new SyncStravaActivityCreateCommand(message.Id, evt.Object_Id, evt.Owner_Id), ct),
                "update" => await sender.Send(
                    new SyncStravaActivityUpdateCommand(message.Id, evt.Object_Id, evt.Owner_Id), ct),
                "delete" => await sender.Send(
                    new SyncStravaActivityDeleteCommand(message.Id, evt.Object_Id), ct),
                _ => throw new InvalidOperationException($"Unknown aspect_type: {evt.Aspect_Type}")
            };

            return result.IsSuccess;
        }

        // athlete deauthorize
        if (evt.Object_Type == "athlete")
        {
            var result = await sender.Send(new SyncStravaAthleteDeauthorizeCommand(message.Id, evt.Owner_Id), ct);
            return result.IsSuccess;
        }

        _logger.LogWarning("Unknown object_type: {ObjectType}", evt.Object_Type);
        return true; // skip unknown types gracefully
    }

    private async Task HandleFailureAsync(QueueMessage message, string error, CancellationToken ct, Exception? ex = null)
    {
        using var scope = _scopeFactory.CreateScope();
        var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();

        await eventRepository.UpdateEventStatusAsync(message.Id, "Failed", error, ct);

        if (message.Attempts >= MaxAttempts)
        {
            _logger.LogWarning("Event {EventId} reached max attempts ({Max}), moving to failed queue", message.Id, MaxAttempts);
            await _redisQueue.MoveToFailedAsync(message, ex ?? new Exception(error), ct);
        }
        else
        {
            _logger.LogInformation("Requeueing event {EventId} (attempt {Attempt})", message.Id, message.Attempts + 1);
            await _redisQueue.PushToQueueAsync(
                RedisQueueService.ActivityQueue,
                message with { Attempts = message.Attempts + 1 },
                ct);
        }
    }
}
