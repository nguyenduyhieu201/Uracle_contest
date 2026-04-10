
namespace Uracle.Application.Abstractions.Interfaces
{
    public interface ITeamRepository
    {
        Task DeleteByContestIdAsync(string contestId, CancellationToken cancellationToken);
        Task<List<Team>> GetByContestIdAsync(string contestId, CancellationToken cancellationToken);
        Task<Team> AddAsync(Team team, CancellationToken cancellationToken);
        Task<Team> GetByIdAsync(string teamId, CancellationToken cancellationToken);
        Task UpdateAsync(Team team, CancellationToken cancellationToken);
        Task<List<Team>> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}
