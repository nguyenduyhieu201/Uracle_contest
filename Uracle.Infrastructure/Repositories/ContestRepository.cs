using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.Contests;

namespace Uracle.Infrastructure.Repositories
{
    public class ContestRepository : IContestRepository
    {
        private ApplicationDbContext _dbContext;
        public ContestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddContest(Contest contest)
        {
            await _dbContext.Contests.AddAsync(contest);
            await _dbContext.SaveChangesAsync();
        }

        public Task<Result<Contest>> GetAllContestAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Contest>> GetByIdAsync(string contestId, CancellationToken cancellationToken)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(contest => contest.Id == contestId);
            if (contest == null)             
            {
                return Result<Contest>.Fail("Contest not found");
            }
            return Result<Contest>.Success(contest);
        }
    }
}
