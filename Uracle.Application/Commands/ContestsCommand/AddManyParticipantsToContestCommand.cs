using Uracle.Application.DTOs.ContestsDto;
using Uracle.Domain.Enums;

namespace Uracle.Application.Commands.ContestsCommand
{
    public sealed record AddMultipleParticipantsCommand (
        string ContestId,
        string token
        ,
        List<string> ParticipantIds
    ) : ICommand<Result<AddMultipleParticipantsResultDto>>;

    public sealed class AddMultipleParticipantsCommandHandler
    : ICommandHandler<AddMultipleParticipantsCommand, Result<AddMultipleParticipantsResultDto>>
    {
        private readonly IContestRepository _contestRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;
        public AddMultipleParticipantsCommandHandler(
            IContestRepository contestRepository,
            IGroupRepository groupRepository,
            IJwtService jwtService
            )
        {
            _contestRepository = contestRepository;
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }
        public async Task<Result<AddMultipleParticipantsResultDto>> Handle(
            AddMultipleParticipantsCommand request,
            CancellationToken cancellationToken)

        {   
            var userId = await _jwtService.ValidateUserAsync(request.token);
            if (userId.IsFail)
            {
                return Result<AddMultipleParticipantsResultDto>.Fail("Invalid token", ErrorCode.Unauthorized);
            }
            var groupId = await _contestRepository.GetGroupByContestAsync(request.ContestId, cancellationToken);
            if (groupId == null)
            {
                return Result<AddMultipleParticipantsResultDto>.Fail("Group of contest not found", ErrorCode.NotFound);
            }
            // 1. Check quyền (group admin/owner)
            var canManage = await _groupRepository
                .CanManageContestAsync(groupId, userId.Value, cancellationToken);
            if (!canManage)
            {
                return Result<AddMultipleParticipantsResultDto>.Fail(
                    "Access denied. Only group admins can add participants to contests.", ErrorCode.Forbidden);
            }
            // 2. Lấy contest
            var contest = await _contestRepository.GetContestByIdAsync(request.ContestId, cancellationToken);
            if (contest is null)
            {
                return Result<AddMultipleParticipantsResultDto>.Fail("Contest not found", ErrorCode.NotFound);
            }
            if (contest.ContestType != ContestType.Individual)
            {
                return Result<AddMultipleParticipantsResultDto>.Fail(
                    "Can only add participants to individual contests", ErrorCode.BadRequest);
            }
            if (contest.HasStarted(DateTime.UtcNow))
            {
                return Result<AddMultipleParticipantsResultDto>.Fail(
                    "Cannot add participants after contest has started", ErrorCode.BadRequest);
            }
            // 3. Xử lý từng participant
            var successful = new List<string>();
            var failed = new List<AddMultipleParticipantsFailedItemDto>();
            foreach (var participantId in request.ParticipantIds)
            {

                var isExists = await _contestRepository
                    .IsUserInContestAsync(request.ContestId, participantId, cancellationToken);
                if (isExists)
                    {
                    failed.Add(new AddMultipleParticipantsFailedItemDto(
                        participantId,
                        "Participant already in contest."));
                    continue;
                }
                var contestUser = ContestUser.Create(request.ContestId, participantId);
                if (contestUser.IsFail)
                {
                    failed.Add(new AddMultipleParticipantsFailedItemDto(
                        participantId,
                        contestUser.Message ?? "Failed to create contest user."));
                    continue;
                }
                await _contestRepository.AddContestUserAsync(contestUser.Value, cancellationToken);
                successful.Add(participantId);

            }
            var resultDto = new AddMultipleParticipantsResultDto(
                successful,
                failed);
            return Result<AddMultipleParticipantsResultDto>.Success(resultDto);
        }
    }
}
