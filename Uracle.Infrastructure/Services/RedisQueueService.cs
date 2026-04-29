// File: Uracle.Infrastructure/Services/RedisQueueService.cs
using StackExchange.Redis;
using System.Text.Json;
using Uracle.Application.Abstractions.Interfaces;

namespace Uracle.Infrastructure.Services;

public class RedisQueueService : IRedisQueueService, IDisposable
{
    private ConnectionMultiplexer? _redis;
    private IDatabase? _database;
    private bool _isConnected;

    public const string ActivityQueue = "activity:queue";
    public const string FailedQueue = "activity:failed";

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_isConnected) return;

        var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_URL")
            ?? "localhost:6379";

        _redis = await ConnectionMultiplexer.ConnectAsync(redisConnectionString);
        _database = _redis.GetDatabase();
        _isConnected = true;
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_redis != null)
        {
            await _redis.DisposeAsync();
            _isConnected = false;
        }
    }

    public async Task<string> PushToQueueAsync(string queue, QueueMessage message, CancellationToken cancellationToken = default)
    {
        EnsureConnected();

        var json = JsonSerializer.Serialize(message);
        await _database!.ListLeftPushAsync(queue, json);

        return message.Id;
    }

    public async Task<QueueMessage?> PopFromQueueAsync(string queue, int timeoutSeconds = 5, CancellationToken cancellationToken = default)
    {
        EnsureConnected();

        // Use blocking pop with timeout (BRPOP equivalent)
        var result = await _database!.ListRightPopAsync(queue);

        if (result.IsNullOrEmpty)
        {
            // No message, wait a bit before next poll
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            return null;
        }

        try
        {
            var json = result.ToString();
            var message = JsonSerializer.Deserialize<QueueMessage>(json);
            return message;
        }
        catch
        {
            return null;
        }
    }

    public async Task MoveToFailedAsync(QueueMessage message, Exception error, CancellationToken cancellationToken = default)
    {
        EnsureConnected();

        var failedData = new
        {
            Message = message,
            Error = error.Message,
            StackTrace = error.StackTrace,
            FailedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        var json = JsonSerializer.Serialize(failedData);
        await _database!.ListLeftPushAsync(FailedQueue, json);
    }

    public async Task<long> GetQueueLengthAsync(string queue, CancellationToken cancellationToken = default)
    {
        EnsureConnected();
        return await _database!.ListLengthAsync(queue);
    }

    private void EnsureConnected()
    {
        if (!_isConnected || _database == null)
        {
            throw new InvalidOperationException("Redis not connected. Call ConnectAsync first.");
        }
    }

    public void Dispose()
    {
        _redis?.Dispose();
    }
}