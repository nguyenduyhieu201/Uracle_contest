
using Uracle.Application.DTOs.IndividualContestActivityDto;
using Uracle.Domain.Models.Contests;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IContestRepository
    {
        Task AddContest(Contest contest);
        Task<Contest> GetContestByIdAsync(string contestId, CancellationToken cancellationToken);
        //Task<Result<Contest>> GetAllContestAsync(strisng userId);
        Task<List<Contest>> GetByGroupIdsAsync(List<string> groupIds, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Contest contest, CancellationToken cancellationToken);
        Task UpdateAsync(Contest contest, CancellationToken cancellationToken);
        Task<List<Contest>> GetByGroupIdAsync(string groupId, CancellationToken cancellationToken);
        Task<List<User>> GetParticipantsAsync(string contestId, CancellationToken ct);

        public Task<ContestUser> AddContestUserAsync(ContestUser contestUser, CancellationToken cancellationToken = default);
        Task<List<User>> GetAllParticipantsInContestAsync(string contestId);
        Task<bool> IsUserInContestAsync(string contestId, string participantId, CancellationToken cancellationToken);
        Task RemoveContestUserAsync(string contestId, string participantId, CancellationToken cancellationToken);

        public Task<bool> DeleteIndividualContestActivityByContestId(string contestId, CancellationToken cancellationToken);
        Task<List<IndividualContestActivity>> GetByContestAndUserAsync(string contestId, string userId, CancellationToken cancellationToken);
        Task<List<IndividualContestLeaderboardRow>> GetLeaderboardAsync(string contestId, CancellationToken cancellationToken);
        Task<string> GetGroupByContestAsync(string contestId, CancellationToken cancellationToken);

        // Activity-sync helpers
        Task<List<(Contest contest, string? teamId)>> GetActiveContestsForUserAsync(string userId, DateTime now, CancellationToken ct = default);
        Task AddIndividualContestActivityAsync(IndividualContestActivity activity, CancellationToken ct = default);
        Task UpdateIndividualActivitiesByWorkoutIdAsync(string workoutActivityId, double distance, int movingTime, string workoutType, double? pace, CancellationToken ct = default);
        Task DeleteIndividualActivitiesByWorkoutIdAsync(string workoutActivityId, CancellationToken ct = default);
    }
}
