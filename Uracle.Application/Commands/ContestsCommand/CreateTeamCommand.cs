using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.Commands.ContestsCommand
{
    public record CreateTeamCommand(string ContestId, string TeamName, string Description) : ICommand<Result<Team>>;

    public class CreateTeamCommandHandler : ICommandHandler<CreateTeamCommand, Result<Team>>
    {
        private readonly IContestRepository _contestRepository;
        private readonly ITeamRepository _teamRepository;
        public CreateTeamCommandHandler(IContestRepository contestRepository, ITeamRepository teamRepository)
        {
            _contestRepository = contestRepository;
            _teamRepository = teamRepository;
        }
        public async Task<Result<Team>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var contest = await _contestRepository.GetContestByIdAsync(request.ContestId, cancellationToken);
            if (contest == null)
            {
                return Result<Team>.Fail("Contest not found", ErrorCode.NotFound);
            }
            if (contest.HasStarted(DateTime.UtcNow))
            {
                return Result<Team>.Fail("Cannot create team after contest has started",ErrorCode.BadRequest);
            }
            var team = Team.Create(
                    groupId: contest.GroupId,
                    contestId: contest.Id,
                    name: request.TeamName
                );
            contest.NumberOfTeams += 1;
            await _teamRepository.AddAsync(team, cancellationToken);
            await _contestRepository.UpdateAsync(contest, cancellationToken);
            return Result<Team>.Success(team);
        }
    }
}
