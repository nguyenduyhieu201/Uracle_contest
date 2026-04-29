// File: Uracle.Infrastructure/Repositories/WorkoutActivityRepository.cs

namespace Uracle.Infrastructure.Repositories;

public class WorkoutActivityRepository : IWorkoutActivityRepository
{
    private readonly ApplicationDbContext _context;

    public WorkoutActivityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkoutActivity?> GetByStravaActivityIdAsync(long stravaActivityId, CancellationToken ct = default)
    {
        return await _context.WorkoutActivities
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.StravaActivityId == stravaActivityId, ct);
    }

    public async Task<WorkoutActivity> CreateAsync(WorkoutActivity activity, CancellationToken ct = default)
    {
        _context.WorkoutActivities.Add(activity);
        await _context.SaveChangesAsync(ct);
        return activity;
    }

    public async Task UpdateAsync(WorkoutActivity activity, CancellationToken ct = default)
    {
        if (_context.Entry(activity).State == EntityState.Detached)
            _context.WorkoutActivities.Attach(activity);

        _context.Entry(activity).State = EntityState.Modified;
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteByStravaActivityIdAsync(long stravaActivityId, CancellationToken ct = default)
    {
        var deleted = await _context.WorkoutActivities
            .Where(w => w.StravaActivityId == stravaActivityId)
            .ExecuteDeleteAsync(ct);
        return deleted > 0;
    }
}
