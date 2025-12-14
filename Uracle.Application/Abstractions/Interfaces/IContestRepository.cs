
using Uracle.Domain.Models.Contests;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IContestRepository
    {
        Task AddContest(Contest contest);
        Task<Result<Contest>> GetByIdAsync(string contestId, CancellationToken cancellationToken);
        Task<Result<Contest>> GetAllContestAsync(string userId);
    }
}
