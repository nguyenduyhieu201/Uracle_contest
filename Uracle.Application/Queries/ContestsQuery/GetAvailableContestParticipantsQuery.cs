using Uracle.Domain.Enums;

namespace Uracle.Application.Queries.ContestsQuery
{
    public sealed record GetAvailableContestParticipantsQuery(string ContestId)
    : IQuery<Result<List<AvailableContestParticipantDto>>>;


    public sealed class GetAvailableContestParticipantsQueryHandler
    : IQueryHandler<GetAvailableContestParticipantsQuery, Result<List<AvailableContestParticipantDto>>>
    {
        private readonly IContestRepository _contestRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        public GetAvailableContestParticipantsQueryHandler(
            IContestRepository contestRepository,
            IGroupRepository groupRepository,
            IUserRepository userRepository)
        {
            _contestRepository = contestRepository;
            _groupRepository = groupRepository;
            _userRepository = userRepository;
        }
        public async Task<Result<List<AvailableContestParticipantDto>>> Handle(
            GetAvailableContestParticipantsQuery request,
            CancellationToken cancellationToken)
        {
            var contest = await _contestRepository.GetContestByIdAsync(request.ContestId, cancellationToken);
            if (contest is null)
            {
                return Result<List<AvailableContestParticipantDto>>
                    .Fail("Contest not found", ErrorCode.NotFound);
            }
            if (contest.ContestType != ContestType.Individual)
            {
                return Result<List<AvailableContestParticipantDto>>
                    .Fail("Contest is not an individual contest", ErrorCode.BadRequest);
            }
            var groupMembers = await _groupRepository.GetByGroupIdAsync(contest.GroupId, cancellationToken);
            var currentParticipants = await _contestRepository.GetAllParticipantsInContestAsync(request.ContestId);
            var usersNotInContest = groupMembers
                        .ExceptBy(
                            currentParticipants.Select(u => u.Id),  // key để so sánh
                            u => u.Id                                // key selector từ groupMembers
                        )
                        .Select(dto => new AvailableContestParticipantDto(
                            dto.Id,
                            dto.Username,
                            dto.Username,
                            dto.StravaProfile is null
                                ? null
                                : new StravaProfileDto(
                                    dto.StravaProfile.Firstname,
                                    dto.StravaProfile.Lastname)))
                        .ToList();

            return Result<List<AvailableContestParticipantDto>>
                .Success(usersNotInContest);
        }
    }
}
