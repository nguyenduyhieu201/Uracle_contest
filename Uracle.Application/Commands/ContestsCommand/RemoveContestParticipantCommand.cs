using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.Abstractions.Security;
using Uracle.Application.DTOs.ContestDto;

namespace Uracle.Application.Commands.ContestsCommand
{
    public record RemoveContestParticipantCommand   
    ( string ContestId,
        string ParticipantId,
        string token
    ) : ICommand<Result<Unit>>;

    public sealed class RemoveContestParticipantCommandHandler
    : ICommandHandler<RemoveContestParticipantCommand, Result<Unit>>
    {
        private readonly IContestRepository _contestRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;
        private readonly IUserTeamsCacheService _userTeamsCacheService;
        public RemoveContestParticipantCommandHandler(
            IContestRepository contestRepository,
            IGroupRepository groupRepository,
            IJwtService jwtService,             
            IUserTeamsCacheService userTeamsCacheService)
        {
            _contestRepository = contestRepository;
            _groupRepository = groupRepository;
            _jwtService = jwtService;
            _userTeamsCacheService = userTeamsCacheService;
        }
        public async Task<Result<Unit>> Handle(
            RemoveContestParticipantCommand request,
            CancellationToken cancellationToken)
        {
            var userId = await _jwtService.ValidateUserAsync(request.token);
            if (userId.IsFail)
            {
                return Result<Unit>.Fail("Invalid token", ErrorCode.Unauthorized);
            }
            var contest = await _contestRepository.GetContestByIdAsync(request.ContestId, cancellationToken);
            if (contest is null)
            {
                return Result<Unit>.Fail("Contest not found.", ErrorCode.NotFound);
            }
            var canRemove = await _groupRepository.CanManageContestAsync(contest.GroupId, userId.Value, cancellationToken);
            if (!canRemove)
            {
                return Result<Unit>.Fail("User does not have permission to remove participant from this contest.", ErrorCode.Forbidden);
            }
            var exists = await _contestRepository.IsUserInContestAsync(request.ContestId, request.ParticipantId, cancellationToken);
            if (!exists)
            {
                return Result<Unit>.Fail("Participant not found in contest.", ErrorCode.NotFound);
            }
            contest.DecreaseParticipants();
            await _contestRepository.RemoveContestUserAsync(request.ContestId, request.ParticipantId, cancellationToken);
            await _contestRepository.UpdateAsync(contest, cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
