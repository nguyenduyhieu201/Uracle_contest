using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.Contests;

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
            string ContestId,
            string CurrentUserId // để check quyền nếu cần
        ) : IQuery<Result<ContestDetailDto>>;
    public class GetContestByIdQueryHandler
    : IQueryHandler<GetContestByIdQuery, Result<ContestDetailDto>>
    {
        private readonly IContestRepository _contestReadRepository;
        private readonly IGroupRepository _groupAuthorization;

        public GetContestByIdQueryHandler(
            IContestRepository contestReadRepository,
            IGroupRepository groupAuthorization)
        {
            _contestReadRepository = contestReadRepository;
            _groupAuthorization = groupAuthorization;
        }

        public async Task<Result<ContestDetailDto>> Handle(
            GetContestByIdQuery request,
            CancellationToken cancellationToken)
        {
            var contest = await _contestReadRepository.GetByIdAsync(request.ContestId, cancellationToken);

            if (contest.IsFail)
            {
                return Result<ContestDetailDto>.Fail("Failed to get Contest detail");
            }

            var contestDetailDto =  new ContestDetailDto(
                contest.Value.Id,
                contest.Value.GroupId,
                contest.Value.Name,
                contest.Value.Detail,
                contest.Value.StartAt,
                contest.Value.EndAt,
                contest.Value.ContestType,
                contest.Value.ActivityType,
                contest.Value.NumberOfTeams,
                contest.Value.NumberOfParticipants
            );
            return Result<ContestDetailDto>.Success(contestDetailDto);
        }
    }
}
