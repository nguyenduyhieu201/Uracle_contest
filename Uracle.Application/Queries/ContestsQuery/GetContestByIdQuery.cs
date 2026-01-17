
using Uracle.Domain.Enums;

namespace Uracle.Application.Queries.ContestsQuery
{
    // Application/Contests/Queries/GetContestById/ContestDetailDto.cs
    public record ContestDetailDto(
        string Id,
        string GroupId,
        string Name,
        string Detail,
        DateTime StartAt,
        DateTime EndAt,
        ContestType ContestType,
        ActivityType ActivityType,
        int NumberOfTeams,
        int NumberOfParticipants
    );
    public record GetContestByIdQuery(
            string ContestId
        ) : IQuery<Result<ContestDetailDto>>;
    public class GetContestByIdQueryHandler
    : IQueryHandler<GetContestByIdQuery, Result<ContestDetailDto>>
    {
        private readonly IContestRepository _contestReadRepository;

        public GetContestByIdQueryHandler(
            IContestRepository contestReadRepository)
        {
            _contestReadRepository = contestReadRepository;
        }

        public async Task<Result<ContestDetailDto>> Handle(
            GetContestByIdQuery request,
            CancellationToken cancellationToken)
        {
            var contest = await _contestReadRepository.GetContestByIdAsync(request.ContestId, cancellationToken);

            if (contest == null)
            {
                return Result<ContestDetailDto>.Fail("Failed to get Contest detail", ErrorCode.NotFound);
            }

            var contestDetailDto =  new ContestDetailDto(
                contest.Id,
                contest.GroupId,
                contest.Name,
                contest.Detail,
                contest.StartAt,
                contest.EndAt,
                contest.ContestType,
                contest.ActivityType,
                contest.NumberOfTeams,
                contest.NumberOfParticipants
            );
            return Result<ContestDetailDto>.Success(contestDetailDto);
        }
    }
}
