


using Uracle.Application.Commands.ContestsCommand;

namespace Uracle.Application.Commands.TeamsCommand
{
    public sealed record AddTeamMembersBulkCommand(
        string TeamId,
        List<string> UserIds
        ) : ICommand<Result<AddTeamMembersBulkResponse>>;
    public class AddTeamMembersBulkCommandValidator : AbstractValidator<AddTeamMembersBulkCommand>
    {
        public AddTeamMembersBulkCommandValidator()
        {
            RuleFor(x => x.TeamId)
                        .NotNull()
                        .WithMessage("Team data is required.");
            RuleFor(x => x.UserIds)
                .Must(list => list is not null && list.Any())
                .WithMessage("UserIds must be a non-empty collection.");
        }
    }
    public class AddTeamMembersBulkCommandHandler : ICommandHandler<AddTeamMembersBulkCommand, Result<AddTeamMembersBulkResponse>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserTeamsCacheService _userTeamsCacheService;
        private readonly IContestRepository _contestRepository;
        public AddTeamMembersBulkCommandHandler(ITeamRepository teamRepository, IContestRepository contestRepository, IUserTeamsCacheService userTeamsCacheService)
        {
            _teamRepository = teamRepository;
            _contestRepository = contestRepository;
            _userTeamsCacheService = userTeamsCacheService;
        }
        public async Task<Result<AddTeamMembersBulkResponse>> Handle(AddTeamMembersBulkCommand request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
            if (team == null)
            {
                return Result<AddTeamMembersBulkResponse>.Fail("Team not found.", ErrorCode.NotFound);
            }
            var contest = await _contestRepository.GetContestByIdAsync(team.ContestId, cancellationToken);
            if (contest == null)
            {
                return Result<AddTeamMembersBulkResponse>.Fail("Contest not found.", ErrorCode.NotFound);
            }
            if (contest.HasStarted(DateTime.UtcNow))
            {
                return Result<AddTeamMembersBulkResponse>.Fail("Cannot add members to team after the contest has started.", ErrorCode.Conflict);
            }
            var successful = new List<string>();
            var failed = new List<string>();
            foreach (var userId in request.UserIds)
            {
                var addResult = team.AddMember(userId);
                if (addResult.IsSuccess)
                {
                    successful.Add(userId);
                }
                else
                {
                    failed.Add(userId);
                    continue;
                    // Có thể log thêm lý do addResult.Error
                }
                await _userTeamsCacheService.CacheUserTeamsAsync(userId);
            }
            await _teamRepository.UpdateAsync(team, cancellationToken);
            contest.IncreaseParticipants(successful.Count);
            await _contestRepository.UpdateAsync(contest, cancellationToken);
            var response = new AddTeamMembersBulkResponse
            {
                Successful = successful,
                Failed = failed
            };
            return Result<AddTeamMembersBulkResponse>.Success(response);
        }
    }
}
