
namespace Uracle.Infrastructure.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private ApplicationDbContext _context;
        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Team> AddAsync(Team team, CancellationToken cancellationToken)
        {
            await _context.Teams.AddAsync(team, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return team;
        }

        public async Task DeleteByContestIdAsync(string contestId, CancellationToken cancellationToken)
        {
            // Lấy tất cả team thuộc contest
            var teams = await _context.Teams
                .Where(t => t.ContestId == contestId)
                .ToListAsync(cancellationToken);

            if (teams.Count == 0)
            {
                return;
            }

            _context.Teams.RemoveRange(teams);
        }

        public async Task<List<Team>> GetByContestIdAsync(string contestId, CancellationToken cancellationToken)
        {
            return await _context.Teams
                .Where(t => t.ContestId == contestId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Team> GetByIdAsync(string teamId, CancellationToken cancellationToken)
        {
            return await _context.Teams
                    .Include(t => t.TeamMembers)      // Load TeamMembers
                    .ThenInclude(tm => tm.User)        // Load User info cho mỗi member
                    .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);
        }

        public async Task<List<Team>> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var teams = await _context.Teams.Where(t => t.TeamMembers.Any(m => m.UserId == userId))
                                .ToListAsync(cancellationToken);
            return teams;
        }

        public async Task UpdateAsync(Team team, CancellationToken cancellationToken)
        {
            // Nếu entity chưa được track, attach vào context
            if (_context.Entry(team).State == EntityState.Detached)
            {
                _context.Teams.Attach(team);
            }
            // Đánh dấu là Modified để EF Core generate câu lệnh UPDATE
            _context.Entry(team).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }


    }
}
