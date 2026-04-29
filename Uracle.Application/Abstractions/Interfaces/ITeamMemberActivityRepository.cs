
namespace Uracle.Application.Abstractions.Interfaces
{
    public interface ITeamMemberActivityRepository
    {
        public Task DeleteByContestIdAsync(string contestId, CancellationToken cancellationToken);
        Task<List<TeamMemberActivity>> FindByTeamAndUserAsync(string teamId, string userId, CancellationToken cancellationToken);
        public Task<List<TeamMemberActivity>> GetByContestIdAsync(string contestId, CancellationToken ct = default);
        Task<List<TeamMemberActivity>> GetByTeamAndContestAsync(string teamId, string contestId, CancellationToken cancellationToken);

        // Activity-sync helpers
        Task CreateAsync(TeamMemberActivity activity, CancellationToken ct = default);
        Task UpdateByWorkoutActivityIdAsync(string workoutActivityId, double distance, int movingTime, string workoutType, double? pace, CancellationToken ct = default);
        Task DeleteByWorkoutActivityIdAsync(string workoutActivityId, CancellationToken ct = default);
    }
}
