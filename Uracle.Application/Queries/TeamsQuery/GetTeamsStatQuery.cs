namespace Uracle.Application.Queries.TeamsQuery
{
    public class GetTeamStatsQuery : IQuery<Result<GetTeamStatsResponse>>
    {
        public string TeamId { get; set; }
    }

    public class GetTeamStatsResponse
    {
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string ContestId { get; set; }
        public TeamStatsDetailDto Stats { get; set; }
        public TeamRankingDto Ranking { get; set; }
        public DateTime CachedAt { get; set; }
    }

    public class GetTeamStatsQueryHandler : IQueryHandler<GetTeamStatsQuery, Result<GetTeamStatsResponse>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberActivityRepository _teamMemberActivityRepository;
        public GetTeamStatsQueryHandler(ITeamRepository teamRepository,  ITeamMemberActivityRepository teamMemberActivityRepository)
        {
            _teamRepository = teamRepository;
            _teamMemberActivityRepository = teamMemberActivityRepository;
        }
        public async Task<Result<GetTeamStatsResponse>> Handle(GetTeamStatsQuery request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
            if (team == null)
            {
                return Result<GetTeamStatsResponse>.Fail("Team not found",ErrorCode.NotFound);
            }
            //var teamStats = await _teamStatsAggregationService.GetIndividualTeamStatsAsync(team.ContestId, request.TeamId);
            //var teamRanking = await _teamStatsAggregationService.CalculateTeamRankings(team.ContestId, request.TeamId);
            var teamStats = await GetIndividualTeamStatsAsync(team.ContestId, request.TeamId, cancellationToken);
            if (teamStats.IsFail) return Result<GetTeamStatsResponse>.Fail(teamStats.Message, teamStats.ErrorCode);
            var teamRanking = await CalculateTeamRankings(team.ContestId, request.TeamId, cancellationToken);
            var response = new GetTeamStatsResponse
            {
                TeamId = request.TeamId,
                TeamName = team.Name,
                ContestId = team.ContestId,
                Stats = teamStats.Value.Stats,
                Ranking = teamRanking,
                CachedAt = DateTime.UtcNow
            };
            return Result<GetTeamStatsResponse>.Success(response);
        }

        public async Task<Result<TeamStatsDto>> GetIndividualTeamStatsAsync(string contestId, string teamId, CancellationToken cancellationToken = default)
        {
            var activities = await _teamMemberActivityRepository.GetByTeamAndContestAsync(teamId, contestId, cancellationToken);
            if (!activities.Any())
            {
                return Result<TeamStatsDto>.Fail("Cannot find any activities", ErrorCode.NotFound);
            }
            var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);
            if (team == null)
            {
                return Result<TeamStatsDto>.Fail("Team not found", ErrorCode.NotFound);
            }
            var stats = CalculateStats(team, activities);
            var ranking = await CalculateTeamRankings(contestId, teamId, cancellationToken);
            var result = new TeamStatsDto
            {
                TeamId = teamId,
                TeamName = team.Name, // Would need to get from team repository
                ContestId = contestId,
                Stats = stats,
                Ranking = ranking,
                CachedAt = DateTime.UtcNow
            };
            return Result<TeamStatsDto>.Success(result);
        }
        private static TeamStatsDetailDto CalculateStats(Team team, List<TeamMemberActivity> activities)
        {

            var totalDistance = activities.Sum(a => a.Distance);
            var totalActivities = activities.Count;
            var memberCount = activities.Select(a => a.UserId).Distinct().Count();

            // Calculate average pace (weighted by distance)
            var averagePace = totalDistance > 0
                ? activities.Where(a => a.Distance > 0).Sum(a => a.Pace * a.Distance) / totalDistance
                : 0;

            var fastestPace = activities.Where(a => a.Pace > 0).Max(a => a.Pace);
            var maxDistance = activities.Max(a => a.Distance);

            // Calculate tracklog (unique user-date combinations)
            var tracklog = activities
                .GroupBy(a => new { a.UserId, Date = a.StartDate.Date })
                .Count();
            return new TeamStatsDetailDto
            {
                TeamName = team.Name,
                TotalDistance = Math.Round(totalDistance, 2),
                AveragePace = Math.Round(averagePace ?? 0, 2),
                TotalTracklog = tracklog,
                FastestPace = Math.Round(fastestPace ?? 0, 2),
                MaxDistance = Math.Round(maxDistance, 2),
                TotalActivities = totalActivities,
                MemberCount = memberCount
            };
        }

        public async Task<Result<TeamStatsDetailDto>> CalculateSingleTeamStats(string contestId, string teamId, CancellationToken ct)
        {
            // Get team info first to ensure team exists
            var team = await _teamRepository.GetByIdAsync(teamId, ct);
            if (team == null)
                return Result<TeamStatsDetailDto>.Fail("Team not found", ErrorCode.NotFound);
            // Verify team belongs to this contest
            if (team.ContestId != contestId)
                return Result<TeamStatsDetailDto>.Fail("Team does not belong to the specified contest", ErrorCode.BadRequest);
            // Get all activities for this team and contest
            var activities = await _teamMemberActivityRepository.GetByTeamAndContestAsync(teamId, contestId, ct);
            if (!activities.Any())
            {
                var detailDto = new TeamStatsDetailDto
                {
                    TeamName = team.Name,
                    TotalDistance = 0,
                    AveragePace = 0,
                    TotalTracklog = 0,
                    FastestPace = 0,
                    MaxDistance = 0,
                    TotalActivities = 0,
                    MemberCount = team.NumberOfMembers
                };
                return Result<TeamStatsDetailDto>.Success(detailDto);
            }
            // Calculate statistics
            var totalDistance = activities.Sum(a => a.Distance);
            var totalActivities = activities.Count;

            // Calculate weighted average pace (only for valid paces)
            var validPaceActivities = activities.Where(a => a.Pace.HasValue && a.Pace > 0 && a.Pace < 999);
            var averagePace = validPaceActivities.Any()
                ? validPaceActivities.Sum(a => a.Pace.Value * a.Distance) / validPaceActivities.Sum(a => a.Distance)
                : 0;

            var fastestPace = validPaceActivities.Any()
                ? validPaceActivities.Min(a => a.Pace.Value)
                : 0;

            var maxDistance = activities.Any()
                ? activities.Max(a => a.Distance)
                : 0;
            // Calculate total tracklog (unique user-date combinations)
            var uniqueTracklogs = activities
                .Select(a => new { a.UserId, Date = a.StartDate.Date })
                .Distinct()
                .Count();
            var teamStatsDetailDto = new TeamStatsDetailDto
            {
                TotalDistance = Math.Round(totalDistance * 100) / 100,
                AveragePace = Math.Round(averagePace * 100) / 100,
                TotalTracklog = uniqueTracklogs,
                FastestPace = Math.Round(fastestPace * 100) / 100,
                MaxDistance = Math.Round(maxDistance * 100) / 100,
                TotalActivities = totalActivities,
                MemberCount = team.NumberOfMembers
            };
            return Result<TeamStatsDetailDto>.Success(teamStatsDetailDto);
        }

        public async Task<TeamRankingDto> CalculateTeamRankings(string contestId, string teamId, CancellationToken ct)
        {
            // Get all teams in this contest
            var allTeamsInContest = await _teamRepository.GetByContestIdAsync(contestId, ct);
            // Calculate stats for all teams
            var teamStatsTasks = allTeamsInContest.Select(async team => new
            {
                TeamId = team.Id,
                Stats = await CalculateSingleTeamStats(contestId, team.Id, ct)
            });
            var allTeamStats = (await Task.WhenAll(teamStatsTasks)).Where(S => S.Stats.IsSuccess);

            // Calculate rankings
            var distanceRanking = allTeamStats
                .OrderByDescending(t => t.Stats.Value.TotalDistance)
                .ToList()
                .FindIndex(t => t.TeamId == teamId) + 1;
            var paceRanking = allTeamStats
                .Where(t => t.Stats.Value.AveragePace > 0) // Only rank teams with valid pace
                .OrderBy(t => t.Stats.Value.AveragePace) // Lower pace is better
                .ToList()
                .FindIndex(t => t.TeamId == teamId) + 1;
            var tracklogRanking = allTeamStats
                .OrderByDescending(t => t.Stats.Value.TotalTracklog)
                .ToList()
                .FindIndex(t => t.TeamId == teamId) + 1;
            return new TeamRankingDto(
                distanceRanking > 0 ? distanceRanking : 999,
                paceRanking > 0 ? paceRanking : 999,
                tracklogRanking > 0 ? tracklogRanking : 999);
        }


    }

}
