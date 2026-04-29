// File: Uracle.API/Endpoints/Stravas/StravaWebhookEndpoint.cs
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Domain.Models;
using Uracle.Infrastructure.Services;
using static Uracle.Application.Abstractions.Interfaces.IRedisQueueService;

namespace Uracle.API.Endpoints.Stravas;

public class StravaWebhookEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // GET /api/webhook - Strava verification
        app.MapGet("api/webhook", (HttpContext ctx, IConfiguration cfg) =>
        {
            var verifyToken = cfg["Strava:WebhookVerifyToken"];
            var mode = ctx.Request.Query["hub.mode"].FirstOrDefault();
            var token = ctx.Request.Query["hub.verify_token"].FirstOrDefault();
            var challenge = ctx.Request.Query["hub.challenge"].FirstOrDefault();

            if (mode == "subscribe" && token == verifyToken && !string.IsNullOrEmpty(challenge))
            {
                return Results.Json(new Dictionary<string, string>
                {
                    ["hub.challenge"] = challenge
                });
            }

            return Results.StatusCode(403);
        });

        // POST /api/webhook - Receive Strava events
        app.MapPost("api/webhook", async (
            HttpContext ctx,
            IRedisQueueService redisQueue,
            IEventRepository eventRepository,
            ILogger<StravaWebhookEndpoint> logger,
            CancellationToken ct) =>
        {
            try
            {
                // Read event body
                ctx.Request.EnableBuffering();
                var body = await new StreamReader(ctx.Request.Body).ReadToEndAsync(ct);
                ctx.Request.Body.Position = 0;

                var webhookEvent = JsonSerializer.Deserialize<StravaWebhookPayload>(body);
                if (webhookEvent == null)
                {
                    return Results.BadRequest("Invalid payload");
                }

                logger.LogInformation(
                    "Received Strava webhook: Type={AspectType}, Object={ObjectType}, ID={ObjectId}, Owner={OwnerId}",
                    webhookEvent.aspect_type,
                    webhookEvent.object_type,
                    webhookEvent.object_id,
                    webhookEvent.owner_id);

                // Store event in SQL Server
                var eventEntity = new WebhookEvent
                {
                    AspectType = webhookEvent.aspect_type,
                    ObjectType = webhookEvent.object_type,
                    ObjectId = webhookEvent.object_id,
                    OwnerId = webhookEvent.owner_id,
                    SubscriptionId = webhookEvent.subscription_id,
                    EventTime = webhookEvent.event_time,
                    Updates = webhookEvent.updates,
                    ReceivedAt = DateTime.UtcNow,
                    Status = "Pending",
                    Attempts = 0
                };

                var eventId = await eventRepository.InsertEventAsync(eventEntity, ct);

                // Push to Redis queue
                var queueMessage = new QueueMessage(
                    eventId.ToString(),
                    new QueueData(
                        webhookEvent.aspect_type,
                        new StravaWebhookEvent(
                            webhookEvent.aspect_type,
                            webhookEvent.object_type,
                            webhookEvent.object_id,
                            webhookEvent.owner_id,
                            webhookEvent.subscription_id,
                            webhookEvent.event_time,
                            webhookEvent.updates)),
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    0);

                await redisQueue.PushToQueueAsync(RedisQueueService.ActivityQueue, queueMessage, ct);

                logger.LogInformation(
                    "Event {EventId} stored and queued successfully",
                    eventId);

                // Return immediately to Strava with 200 OK
                return Results.Ok("OK");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing webhook event");
                // Still return 200 to Strava to avoid retries, but log the error
                return Results.Ok(new { message = "Error processing webhook event, queued for retry" });
            }
        });
    }
}

public class StravaWebhookPayload
{
    public string aspect_type { get; set; } = string.Empty;
    public string object_type { get; set; } = string.Empty;
    public long object_id { get; set; }
    public long owner_id { get; set; }
    public long subscription_id { get; set; }
    public long event_time { get; set; }
    public Dictionary<string, string>? updates { get; set; }
}