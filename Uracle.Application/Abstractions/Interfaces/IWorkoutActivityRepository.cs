namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IWorkoutActivityRepository
    {
        Task<WorkoutActivity?> GetByStravaActivityIdAsync(long stravaActivityId, CancellationToken ct = default);
        Task<WorkoutActivity> CreateAsync(WorkoutActivity activity, CancellationToken ct = default);
        Task UpdateAsync(WorkoutActivity activity, CancellationToken ct = default);
        Task<bool> DeleteByStravaActivityIdAsync(long stravaActivityId, CancellationToken ct = default);
    }
}
