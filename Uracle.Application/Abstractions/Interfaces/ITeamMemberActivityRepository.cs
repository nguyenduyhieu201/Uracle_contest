
namespace Uracle.Application.Abstractions.Interfaces
{
    public interface ITeamMemberActivityRepository 
    {
        public  Task DeleteByContestIdAsync(string contestId, CancellationToken cancellationToken);
        Task<List<TeamMemberActivity>> FindByTeamAndUserAsync(string teamId, string userId, CancellationToken cancellationToken);
        public Task<List<TeamMemberActivity>> GetByContestIdAsync(string contestId, CancellationToken ct = default);
        Task<List<TeamMemberActivity>> GetByTeamAndContestAsync(string teamId, string contestId, CancellationToken cancellationToken);
    }
}
