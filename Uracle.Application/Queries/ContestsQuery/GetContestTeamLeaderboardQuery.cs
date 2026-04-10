using Uracle.Domain.Enums;

namespace Uracle.Application.Queries.ContestsQuery
{
    public sealed record GetContestTeamLeaderboardQuery(
        string ContestId,
        string Metric,
        int Limit
    ) : IQuery<Result<TeamLeaderboardResponseDto>>;

    public sealed class GetContestTeamLeaderboardQueryHandler
    : IQueryHandler<GetContestTeamLeaderboardQuery, Result<TeamLeaderboardResponseDto>>
    {
        private readonly ITeamMemberActivityRepository _teamMemberActivityRepository;
        private readonly IContestRepository _contestRepository;
        private readonly ITeamRepository _teamRepo;

        public GetContestTeamLeaderboardQueryHandler(ITeamMemberActivityRepository teamMemberActivityRepository, ITeamRepository teamRepository, IContestRepository contestRepository)
        {
            _teamMemberActivityRepository = teamMemberActivityRepository;
            _teamRepo = teamRepository;
            _contestRepository = contestRepository;
        }
        public async Task<Result<TeamLeaderboardResponseDto>> Handle(
            GetContestTeamLeaderboardQuery request,
            CancellationToken cancellationToken)
        {
            var contest = await _contestRepository.GetContestByIdAsync(request.ContestId, cancellationToken);
            if (contest is null)
            {
                return Result<TeamLeaderboardResponseDto>.Fail("Contest not found", ErrorCode.NotFound);
            }
            if (contest.ContestType != ContestType.Team)
            {
                return Result<TeamLeaderboardResponseDto>.Fail(
                    "Contest is not Team type", ErrorCode.BadRequest);
            }
            if (!TryParseMetric(request.Metric, out var metric))
            {
                return Result<TeamLeaderboardResponseDto>.Fail(
                    "Invalid metric. Use totalDistance, averagePace, or totalTracklog", ErrorCode.BadRequest);
            }
            if (request.Limit is < 1 or > 1000)
            {
                return Result<TeamLeaderboardResponseDto>.Fail(
                    "Limit must be between 1 and 1000", ErrorCode.BadRequest);
            }
            var result = await GetContestTeamLeaderboardAsync(
                request.ContestId,
                metric,
                request.Limit,
                cancellationToken);


            var dto = new TeamLeaderboardResponseDto(
                result.ContestId,
                MetricToString(result.Metric),
                result.CachedAt,
                result.Teams
                    .Select(t => new TeamLeaderboardItemDto(
                        t.TeamId,
                        t.TeamName,
                        t.TotalDistance,
                        t.AveragePace,
                        t.TotalTracklog,
                        t.FastestPace,
                        t.MaxDistance,
                        t.TotalActivities,
                        t.MemberCount,
                        t.Rank,
                        t.Badge))
                    .ToList());
            return Result<TeamLeaderboardResponseDto>.Success(dto);
        }
        private static bool TryParseMetric(string metric, out TeamLeaderboardMetric value)
        {
            switch (metric)
            {
                case "totalDistance":
                    value = TeamLeaderboardMetric.TotalDistance;
                    return true;
                case "averagePace":
                    value = TeamLeaderboardMetric.AveragePace;
                    return true;
                case "totalTracklog":
                    value = TeamLeaderboardMetric.TotalTracklog;
                    return true;
                default:
                    value = TeamLeaderboardMetric.TotalDistance;
                    return false;
            }
        }
        private static string MetricToString(TeamLeaderboardMetric metric) =>
            metric switch
            {
                TeamLeaderboardMetric.TotalDistance => "totalDistance",
                TeamLeaderboardMetric.AveragePace => "averagePace",
                TeamLeaderboardMetric.TotalTracklog => "totalTracklog",
                _ => "totalDistance"
            };

        private async Task<TeamLeaderboardResult> GetContestTeamLeaderboardAsync (string contestId, TeamLeaderboardMetric metric,
                                                                                    int limit,
                                                                                    CancellationToken ct = default)
        {
            var teams = await _teamRepo.GetByContestIdAsync(contestId, ct);
            if (teams.Count == 0)
            {
                return new TeamLeaderboardResult(
                    contestId,
                    metric,
                    DateTime.UtcNow,
                    new List<TeamLeaderboardItem>());
            }
            var activities = await _teamMemberActivityRepository.GetByContestIdAsync(contestId, ct);
            // 3. Group theo TeamId + join với team để có tên và số thành viên
            var grouped =
                from a in activities
                group a by a.TeamId
                into g
                join t in teams on g.Key equals t.Id
                select new
                {
                    Team = t,
                    TotalDistance = g.Sum(x => x.Distance),
                    TotalActivities = g.Count(),
                    AvgPaceWeighted = g.Sum(x => x.Distance > 0 ? x.Pace * x.Distance : 0),
                    DistForPace = g.Sum(x => x.Distance > 0 ? x.Distance : 0),
                    FastestPace = g.Where(x => x.Pace > 0 && x.Pace < 999).Min(x => (double?)x.Pace),
                    MaxDistance = g.Max(x => x.Distance),
                    // tổng số tracklog: mỗi user mỗi ngày tối đa 1 (giống pipeline Mongo)
                    TotalTracklog = g
                        .GroupBy(x => new { x.UserId, Date = x.StartDate.Date })
                        .Count()
                };
            var aggregated = grouped.ToList();
            // 4. Tính AveragePace + build TeamLeaderboardItem
            var items = aggregated.Select(x =>
            {
                var avgPace = x.DistForPace > 0
                    ? x.AvgPaceWeighted / x.DistForPace
                    : 0;
                return new TeamLeaderboardItem(
                    TeamId: x.Team.Id,
                    TeamName: x.Team.Name,
                    TotalDistance: Math.Round(x.TotalDistance, 2),
                    AveragePace: Math.Round((double)(avgPace ?? 0), 2),
                    TotalTracklog: x.TotalTracklog,
                    FastestPace: Math.Round((x.FastestPace ?? 0), 2),
                    MaxDistance: Math.Round(x.MaxDistance, 2),
                    TotalActivities: x.TotalActivities,
                    MemberCount: x.Team.NumberOfMembers,
                    Rank: 0,       // sẽ set bên dưới
                    Badge: null    // sẽ set bên dưới
                );
            }).ToList();
            // 5. Sort theo metric + limit
            items = SortByMetric(items, metric)
                .Take(limit)
                .ToList();
            // 6. Gán rank + badge (gold/silver/bronze)
            for (var i = 0; i < items.Count; i++)
            {
                var badge = i switch
                {
                    0 => "gold",
                    1 => "silver",
                    2 => "bronze",
                    _ => null
                };
                items[i] = items[i] with
                {
                    Rank = i + 1,
                    Badge = badge
                };
            }
            // 7. Trả về kết quả
            return new TeamLeaderboardResult(
                contestId,
                metric,
                DateTime.UtcNow,
                items);
        }
        private static IEnumerable<TeamLeaderboardItem> SortByMetric(
                                    IEnumerable<TeamLeaderboardItem> items,
                                    TeamLeaderboardMetric metric) =>
        metric switch
        {
            TeamLeaderboardMetric.TotalDistance =>
                items.OrderByDescending(x => x.TotalDistance),
            // Pace: nhỏ hơn là tốt hơn, 0 (không có pace) coi như tệ nhất
            TeamLeaderboardMetric.AveragePace =>
                items.OrderBy(x => x.AveragePace == 0 ? 999 : x.AveragePace),
            TeamLeaderboardMetric.TotalTracklog =>
                items.OrderByDescending(x => x.TotalTracklog),
            _ => items.OrderByDescending(x => x.TotalDistance)
        };
    }
}
