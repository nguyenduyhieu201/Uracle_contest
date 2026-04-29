

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

        public async Task CreateAsync(TeamMemberActivity activity, CancellationToken ct = default)
        {
            _context.TeamMemberActivities.Add(activity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateByWorkoutActivityIdAsync(
            string workoutActivityId, double distance, int movingTime, string workoutType, double? pace, CancellationToken ct = default)
        {
            await _context.TeamMemberActivities
                .Where(a => a.WorkoutActivityId == workoutActivityId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Distance, distance)
                    .SetProperty(a => a.MovingTime, movingTime)
                    .SetProperty(a => a.WorkoutType, workoutType)
                    .SetProperty(a => a.Pace, pace), ct);
        }

        public async Task DeleteByWorkoutActivityIdAsync(string workoutActivityId, CancellationToken ct = default)
        {
            await _context.TeamMemberActivities
                .Where(a => a.WorkoutActivityId == workoutActivityId)
                .ExecuteDeleteAsync(ct);
        }
    }
}
