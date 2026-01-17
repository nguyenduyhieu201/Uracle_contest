using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.TeamMember;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Uracle.Application.Commands.TeamsCommand
{
    public  record AddTeamMemberCommand(string token, string participantId, string teamId) : ICommand<Result<Team>>;
    public class AddTeamMemberCommandHandler : ICommandHandler<AddTeamMemberCommand, Result<Team>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IContestRepository _contestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        public AddTeamMemberCommandHandler(
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            IContestRepository contestRepository,
            IJwtService jwtService
            )
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _contestRepository = contestRepository;
            _jwtService = jwtService;
        }
        public async Task<Result<Team>> Handle(AddTeamMemberCommand request, CancellationToken cancellationToken)
        {
            // Placeholder for adding a team member logic
            //return Result.Success(true);
            var userId = await _jwtService.ValidateUserAsync(request.token, cancellationToken);
            if (!userId.IsSuccess)
            {
                return Result<Team>.Fail("Invalid token", ErrorCode.Unauthorized);
            }
            var participant = await _userRepository.GetUserByIdAsync(request.participantId, cancellationToken);
            if (participant is null)
            {
                return Result<Team>.Fail("Cannot find any appropirate participant", ErrorCode.NotFound);
            }
            var team = await _teamRepository.GetByIdAsync(request.teamId, cancellationToken);
            if (team is null)
            {
                return Result<Team>.Fail("Cannot find any appropirate team", ErrorCode.NotFound);
            }
            var contest = await _contestRepository.GetContestByIdAsync(team.ContestId, cancellationToken);
            if (contest is null)
            {
                return Result<Team>.Fail("Cannot find any appropirate contest", ErrorCode.NotFound);
            }
            var now = DateTime.UtcNow;
            if (contest.HasStarted(now))
            {
                return Result<Team>.Fail("Contest has started", ErrorCode.BadRequest);
            }
            team.AddMember(participant.Id, now);
            contest.IncreaseParticipants();
            await _teamRepository.UpdateAsync(team, cancellationToken);
            await _contestRepository.UpdateAsync(contest, cancellationToken);
            return Result<Team>.Success(team);
        }
    }

}
