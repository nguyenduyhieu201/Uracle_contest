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

        public async Task<bool> DeleteAsync(
            Contest contest,
            CancellationToken cancellationToken = default)
        {
            // Attach nếu entity đang ở trạng thái Detached
            if (_dbContext.Entry(contest).State == EntityState.Detached)
            {
                _dbContext.Contests.Attach(contest);
            }

            _dbContext.Contests.Remove(contest);



            // Ví dụ 1: dùng UnitOfWork ở ngoài → không SaveChanges:
            var changes = await _dbContext.SaveChangesAsync(cancellationToken);
            return changes > 0;
        }

        public async Task<List<Contest>> GetByGroupIdAsync(string groupId, CancellationToken cancellationToken)
        {
            return await _dbContext.Contests
                        .Where(c => c.GroupId == groupId)
                        .ToListAsync(cancellationToken);
        }

        public async Task<List<Contest>> GetByGroupIdsAsync(List<string> groupIds, CancellationToken cancellationToken)
        {
            return await _dbContext.Contests
                .Where(c => groupIds.Contains(c.GroupId))
                .ToListAsync(cancellationToken);
        }

        public async Task<Contest> GetContestByIdAsync(string contestId, CancellationToken cancellationToken)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(contest => contest.Id == contestId);
            return contest;
        }

        public async Task<List<User>> GetParticipantsAsync(string contestId, CancellationToken ct)
        {
            var participants = await (from c in _dbContext.Contests
                                  join cp in _dbContext.ContestUsers
                                  on c.Id equals cp.ContestId
                                  join u in _dbContext.Users
                                  on cp.UserId equals u.Id
                                  where cp.ContestId == contestId
                                  select u).ToListAsync(ct);
            return participants;
        }

        public async Task UpdateAsync(Contest contest, CancellationToken cancellationToken)
        {
            // Nếu entity chưa được track, attach vào context
            if (_dbContext.Entry(contest).State == EntityState.Detached)
            {
                _dbContext.Contests.Attach(contest);
            }
            // Đánh dấu là Modified để EF generate UPDATE
            _dbContext.Entry(contest).State = EntityState.Modified;
             await _dbContext.SaveChangesAsync(cancellationToken);
        }


        public async Task<ContestUser> AddContestUserAsync(ContestUser contestUser, CancellationToken cancellationToken = default)
        {
            await _dbContext.ContestUsers.AddAsync(contestUser, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return contestUser;
        }

        public async Task<List<User>> GetAllParticipantsInContestAsync(string contestId)
        {
            var users = await (_dbContext.ContestUsers
                .Where(cu => cu.ContestId == contestId)
                .Join(_dbContext.Users,
                      cu => cu.UserId,
                      u => u.Id,
                      (cu, u) => u)).ToListAsync();
            return users;
        }

        public Task<bool> IsUserInContestAsync(string contestId, string participantId, CancellationToken cancellationToken)
        {
            return _dbContext.ContestUsers
                .AnyAsync(cu => cu.ContestId == contestId && cu.UserId == participantId, cancellationToken);
        }

        public async Task RemoveContestUserAsync(string contestId, string participantId, CancellationToken cancellationToken)
        {
            await _dbContext.ContestUsers.Where(cu => cu.ContestId == contestId && cu.UserId == participantId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<bool> DeleteIndividualContestActivityByContestId(string contestId, CancellationToken cancellationToken)
        {
            // Lấy tất cả team thuộc contest
            var individualContestActivities = await _dbContext.IndividualContestActivities
                .Where(t => t.ContestId == contestId)
                .ToListAsync(cancellationToken);

            if (individualContestActivities.Count == 0)
            {
                return false;
            }

            _dbContext.IndividualContestActivities.RemoveRange(individualContestActivities);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<IndividualContestActivity>> GetByContestAndUserAsync(string contestId, string userId, CancellationToken cancellationToken)
        {
            return await _dbContext.IndividualContestActivities
                                    .AsNoTracking()
                                    .Where(a => a.ContestId == contestId && a.UserId == userId)
                                    .OrderByDescending(a => a.StartDate)   // giống Node sort mới nhất trước
                                    .ToListAsync(cancellationToken);
        }

        public async Task<List<IndividualContestLeaderboardRow>> GetLeaderboardAsync(string contestId, CancellationToken cancellationToken)
        {
            // group theo UserId và tính các chỉ số aggregate
            var query =
                from a in _dbContext.IndividualContestActivities.AsNoTracking()
                where a.ContestId == contestId
                group a by a.UserId
                into g
                select new
                {
                    UserId = g.Key,
                    // Distance là double => Sum/Max trả về double
                    TotalDistance = g.Sum(x => x.Distance),
                    TotalTracklog = g.Count(),
                    // average pace có trọng số theo distance, bỏ qua record không có Pace hoặc Distance <= 0
                    AveragePaceWeighted = g.Sum(
                        x => x.Distance > 0 && x.Pace.HasValue
                            ? x.Pace.Value * x.Distance
                            : 0
                    ),
                    DistanceForPace = g.Sum(
                        x => x.Distance > 0 && x.Pace.HasValue
                            ? x.Distance
                            : 0
                    ),
                    // fastest pace: nhỏ nhất, bỏ qua giá trị không hợp lệ
                    FastestPace = g
                        .Where(x => x.Pace > 0 && x.Pace < 999)
                        .Min(x => (double?)x.Pace),
                    MaxDistance = g.Max(x => x.Distance)
                };
            var rowsRaw = await query.ToListAsync(cancellationToken);
            var rows = rowsRaw
                .Select(x =>
                {
                    // tính average pace (theo trọng số distance)
                    double averagePace =
                        x.DistanceForPace > 0
                            ? x.AveragePaceWeighted / x.DistanceForPace
                            : 0;
                    return new IndividualContestLeaderboardRow(
                        x.UserId,
                        TotalDistance: Math.Round(x.TotalDistance, 2),
                        TotalTracklog: x.TotalTracklog,
                        AveragePace: Math.Round(averagePace, 2),
                        FastestPace: Math.Round((x.FastestPace ?? 0), 2),
                        MaxDistance: Math.Round(x.MaxDistance, 2)
                    );
                })
                // thường sort theo tổng quãng đường giảm dần
                .OrderByDescending(r => r.TotalDistance)
                .ToList(); 
            return rows;
        }

        public async Task<string> GetGroupByContestAsync(string contestId, CancellationToken cancellationToken)
        {
            return await _dbContext.Contests.Where(c => c.Id == contestId)
                                            .Select(c => c.GroupId)
                                            .FirstOrDefaultAsync(cancellationToken);
        }

        // Returns active contests for a user: individual (teamId = null) + team (teamId = team's Id)
        public async Task<List<(Contest contest, string? teamId)>> GetActiveContestsForUserAsync(
            string userId, DateTime now, CancellationToken ct = default)
        {
            // Individual contests where user is enrolled
            var individualContests = await (
                from c in _dbContext.Contests
                join cu in _dbContext.ContestUsers on c.Id equals cu.ContestId
                where cu.UserId == userId
                   && c.StartAt <= now && c.EndAt >= now
                   && c.ContestType == ContestType.Individual
                select c
            ).ToListAsync(ct);

            // Team contests where user is a team member
            var teamContests = await (
                from c in _dbContext.Contests
                join t in _dbContext.Teams on c.Id equals t.ContestId
                join tm in _dbContext.TeamMembers on t.Id equals tm.TeamId
                where tm.UserId == userId
                   && c.StartAt <= now && c.EndAt >= now
                   && c.ContestType == ContestType.Team
                select new { Contest = c, TeamId = t.Id }
            ).ToListAsync(ct);

            var result = individualContests.Select(c => (c, (string?)null)).ToList();
            result.AddRange(teamContests.Select(x => (x.Contest, (string?)x.TeamId)));
            return result;
        }

        public async Task AddIndividualContestActivityAsync(IndividualContestActivity activity, CancellationToken ct = default)
        {
            _dbContext.IndividualContestActivities.Add(activity);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task UpdateIndividualActivitiesByWorkoutIdAsync(
            string workoutActivityId, double distance, int movingTime, string workoutType, double? pace, CancellationToken ct = default)
        {
            await _dbContext.IndividualContestActivities
                .Where(a => a.WorkoutActivityId == workoutActivityId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Distance, distance)
                    .SetProperty(a => a.MovingTime, movingTime)
                    .SetProperty(a => a.WorkoutType, workoutType)
                    .SetProperty(a => a.Pace, pace), ct);
        }

        public async Task DeleteIndividualActivitiesByWorkoutIdAsync(string workoutActivityId, CancellationToken ct = default)
        {
            await _dbContext.IndividualContestActivities
                .Where(a => a.WorkoutActivityId == workoutActivityId)
                .ExecuteDeleteAsync(ct);
        }
    }
}
