using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IRedisQueueService
    {
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);
        Task<string> PushToQueueAsync(string queue, QueueMessage message, CancellationToken cancellationToken = default);
        Task<QueueMessage?> PopFromQueueAsync(string queue, int timeoutSeconds = 5, CancellationToken cancellationToken = default);
        Task MoveToFailedAsync(QueueMessage message, Exception error, CancellationToken cancellationToken = default);
        Task<long> GetQueueLengthAsync(string queue, CancellationToken cancellationToken = default);
    }

    public record QueueMessage(
        string Id,           // Event ID (GUID as string)
        QueueData Data,
        long Timestamp,
        int Attempts = 0
    );

    public record QueueData(
        string Type,         // create, update, delete
        StravaWebhookEvent Event
    );

    public record StravaWebhookEvent(
        string Aspect_Type,
        string Object_Type,
        long Object_Id,
        long Owner_Id,
        long Subscription_Id,
        long Event_Time,
        Dictionary<string, string>? Updates
    );
}
