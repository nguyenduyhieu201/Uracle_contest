using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.TeamMember;

namespace Uracle.Application.Commands.TeamsCommand
{
    public sealed record RemoveTeamMemberCommand(string TeamId, string participantId, string token) : ICommand<Result<Unit>>;

    public class RemoveTeamMemberCommandHandler : ICommandHandler<RemoveTeamMemberCommand, Result<Unit>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;
        private readonly IContestRepository _contestRepository;
        private readonly IUserTeamsCacheService _userTeamsCacheService;
        public RemoveTeamMemberCommandHandler(ITeamRepository teamRepository, 
                                                IJwtService jwtService, IGroupRepository groupRepository, 
                                                IUserTeamsCacheService userTeamsCacheService, IContestRepository contestRepository)
        {
            _teamRepository = teamRepository;
            _jwtService = jwtService;
            _groupRepository = groupRepository;
            _userTeamsCacheService = userTeamsCacheService;
            _contestRepository = contestRepository;
        }
        public async Task<Result<Unit>> Handle(
            RemoveTeamMemberCommand request,
            CancellationToken cancellationToken)
        {
            var userId = await _jwtService.ValidateUserAsync(request.token, cancellationToken);
            if (!userId.IsSuccess)
            {
                return Result<Unit>.Fail("Invalid token", ErrorCode.Unauthorized);
            }
            var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
            if (team is null)
            {
                return Result<Unit>.Fail("Team not found.", ErrorCode.NotFound);
            }
            var contestId = team.Contest.Id;
            var canManage = await _groupRepository.IsUserAdminInGroup(team.Contest.GroupId, userId.Value, cancellationToken);
            if (!canManage)
            {
                return Result<Unit>.Fail("User is not the admin of the group.", ErrorCode.Forbidden);
            }
            if (team.Contest.HasStarted(DateTime.UtcNow))
            {
                return Result<Unit>.Fail("Cannot remove members from team after the contest has started.", ErrorCode.Conflict);
            }
            var removeResult = team.RemoveMember(userId.Value);
            if (!removeResult.IsSuccess)
            {
                // Có thể log removeResult.Error
                return Result<Unit>.Fail(removeResult.Message!, ErrorCode.InternalError);
            }
            await _teamRepository.UpdateAsync(team, cancellationToken);
            team.Contest.DecreaseParticipants();
            await _userTeamsCacheService.CleanupCacheForUserAsync(request.participantId, cancellationToken);
            await _contestRepository.UpdateAsync(team.Contest, cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
