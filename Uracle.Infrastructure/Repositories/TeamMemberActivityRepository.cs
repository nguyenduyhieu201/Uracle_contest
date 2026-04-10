

namespace Uracle.Infrastructure.Repositories
{
    public class TeamMemberActivityRepository : ITeamMemberActivityRepository
    {
        private readonly ApplicationDbContext _context;
        public TeamMemberActivityRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task DeleteByContestIdAsync(string contestId, CancellationToken cancellationToken)
        {
            var activities = await _context.TeamMemberActivities
                .Where(a => a.ContestId == contestId)
                .ToListAsync(cancellationToken);

            if (activities.Count == 0)
                return;

            _context.TeamMemberActivities.RemoveRange(activities);
        }

        public async Task<List<TeamMemberActivity>> FindByTeamAndUserAsync(string teamId, string userId, CancellationToken cancellationToken)
        {
            var teamMemberActivities = await _context.TeamMemberActivities.AsNoTracking()
                                        .Where(a => a.TeamId == teamId && a.UserId == userId)
                                        .OrderByDescending(a => a.StartDate)
                                        .AsNoTracking()
                                        .ToListAsync();
            return teamMemberActivities;
        }

        public async Task<List<TeamMemberActivity>> GetByContestIdAsync(string contestId, CancellationToken ct = default)
        {
            return await _context.TeamMemberActivities
                        .AsNoTracking()
                        .Where(a => a.ContestId == contestId)
                        .ToListAsync(ct);
        }

        public async Task<List<TeamMemberActivity>> GetByTeamAndContestAsync(string teamId, string contestId, CancellationToken cancellationToken)
        {
            return await _context.TeamMemberActivities
                        .AsNoTracking()
                        .Where(a => a.TeamId == teamId && a.ContestId == contestId)
                        .ToListAsync(cancellationToken);
        }
    }
}
