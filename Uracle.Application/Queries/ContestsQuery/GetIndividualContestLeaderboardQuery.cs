using Uracle.Domain.Enums;

namespace Uracle.Application.Queries.ContestsQuery
{
    public sealed record GetIndividualContestLeaderboardQuery(string ContestId)
    : IQuery<Result<List<IndividualContestLeaderboardItemDto>>>;
    public sealed record IndividualContestLeaderboardItemDto(
        string UserId,
        double TotalDistance,
        int TotalTracklog,
        double AveragePace,
        double FastestPace,
        double MaxDistance,
        UserSummaryDto? UserDetails
    );
    public sealed record UserSummaryDto(
        string UserId,
        string Name,
        string Username,
        StravaProfileDto? StravaProfile
    );
    public sealed class GetIndividualContestLeaderboardQueryHandler
    : IQueryHandler<GetIndividualContestLeaderboardQuery, Result<List<IndividualContestLeaderboardItemDto>>>
    {
        private readonly IContestRepository _contestRepository;
        private readonly IUserRepository _userRepository;
        public GetIndividualContestLeaderboardQueryHandler(
            IContestRepository contestRepository,
            IUserRepository userRepository)
        {
            _contestRepository = contestRepository;
            _userRepository = userRepository;
        }
        public async Task<Result<List<IndividualContestLeaderboardItemDto>>> Handle(
            GetIndividualContestLeaderboardQuery request,
            CancellationToken cancellationToken)
        {
            var contest = await _contestRepository.GetContestByIdAsync(request.ContestId, cancellationToken);
            if (contest is null)
            {
                return Result<List<IndividualContestLeaderboardItemDto>>
                    .Fail("Contest not found", ErrorCode.NotFound);
            }
            if (contest.ContestType != ContestType.Individual)
            {
                return Result<List<IndividualContestLeaderboardItemDto>>
                    .Fail("Contest is not an individual contest", ErrorCode.BadRequest);
            }
            var rows = await _contestRepository.GetLeaderboardAsync(request.ContestId, cancellationToken);
            var result = new List<IndividualContestLeaderboardItemDto>();
            foreach (var row in rows)
            {
                UserSummaryDto? userSummary = null;
                var user = await _userRepository.GetUserByIdAsync(row.UserId, cancellationToken);
                if (user is not null)
                {
                    string name;
                    if (!string.IsNullOrWhiteSpace(user.StravaProfile?.Firstname) &&
                        !string.IsNullOrWhiteSpace(user.StravaProfile?.Lastname))
                    {
                        name = $"{user.StravaProfile!.Firstname} {user.StravaProfile!.Lastname}";
                    }
                    else
                    {
                        name = user.Username;
                    }
                    userSummary = new UserSummaryDto(
                        user.Id,
                        name,
                        user.Username,
                        user.StravaProfile is null
                            ? null
                            : new StravaProfileDto(
                                user.StravaProfile.Firstname,
                                user.StravaProfile.Lastname));
                }
                result.Add(new IndividualContestLeaderboardItemDto(
                    row.UserId,
                    row.TotalDistance,
                    row.TotalTracklog,
                    row.AveragePace,
                    row.FastestPace,
                    row.MaxDistance,
                    userSummary));
            }
            return Result<List<IndividualContestLeaderboardItemDto>>
                .Success(result);
        }
    }
}
