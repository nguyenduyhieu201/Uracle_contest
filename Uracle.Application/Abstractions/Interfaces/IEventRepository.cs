using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IEventRepository
    {
        Task<string> InsertEventAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken = default);
        Task UpdateEventStatusAsync(string eventId, string status, string? errorMessage = null, CancellationToken cancellationToken = default);
        Task IncrementAttemptsAsync(string eventId, CancellationToken cancellationToken = default);
        Task<WebhookEvent?> FindByIdAsync(string eventId, CancellationToken cancellationToken = default);
    }

}
