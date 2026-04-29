// File: Uracle.Infrastructure/Repositories/EventRepository.cs
using Microsoft.EntityFrameworkCore;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Domain.Models;

namespace Uracle.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> InsertEventAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken = default)
    {
        webhookEvent.ReceivedAt = DateTime.UtcNow;
        webhookEvent.Status = "Pending";
        webhookEvent.Attempts = 0;

        _context.WebhookEvents.Add(webhookEvent);
        await _context.SaveChangesAsync(cancellationToken);

        return webhookEvent.Id;
    }

    public async Task UpdateEventStatusAsync(string eventId, string status, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        var webhookEvent = await _context.WebhookEvents.FindAsync(new object[] { eventId }, cancellationToken);

        if (webhookEvent != null)
        {
            webhookEvent.Status = status;
            if (errorMessage != null)
            {
                webhookEvent.ErrorMessage = errorMessage;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task IncrementAttemptsAsync(string eventId, CancellationToken cancellationToken = default)
    {
        var webhookEvent = await _context.WebhookEvents.FindAsync(new object[] { eventId }, cancellationToken);

        if (webhookEvent != null)
        {
            webhookEvent.Attempts++;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<WebhookEvent?> FindByIdAsync(string eventId, CancellationToken cancellationToken = default)
    {
        return await _context.WebhookEvents
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
    }
}