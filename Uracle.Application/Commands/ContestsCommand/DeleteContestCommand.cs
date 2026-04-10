
using Uracle.Application.Abstractions.Interfaces;

namespace Uracle.Application.Commands.ContestsCommand
{
    public record DeleteContestCommand(string contestId, string token) : ICommand<Result<bool>>;
    public class DeleteContestCommandHandler : ICommandHandler<DeleteContestCommand, Result<bool>>
    {
        private IContestRepository _contestRepository;
        private IJwtService _jWTService;
        private IGroupRepository _groupRepository; 
        private ITeamMemberActivityRepository _teamMemberActivityRepository;
        private ITeamRepository _teamRepository;
        private IUserTeamsCacheService _userTeamsCacheService;
        public DeleteContestCommandHandler(IContestRepository contestRepository, 
                                            IJwtService jWTService, 
                                            IGroupRepository groupRepository,
                                            ITeamMemberActivityRepository teamMemberActivityRepository,
                                            ITeamRepository teamRepository,
                                            IUserTeamsCacheService userTeamsCacheService)
        {
            _contestRepository = contestRepository;
            _jWTService = jWTService;
            _groupRepository = groupRepository;
            _teamMemberActivityRepository = teamMemberActivityRepository;
            _teamRepository = teamRepository;
            _userTeamsCacheService = userTeamsCacheService;
        }
        public async Task<Result<bool>> Handle(DeleteContestCommand request, CancellationToken cancellationToken)
        {
            var userId = await _jWTService.ValidateUserAsync(request.token);
            if (userId.IsFail)
            {
                return Result<bool>.Fail("Invalid token", ErrorCode.Unauthorized);
            }
            var contest = await _contestRepository.GetContestByIdAsync(request.contestId, cancellationToken);
            if (contest == null)
            {
                return Result<bool>.Fail("Contest not found", ErrorCode.NotFound);
            }
            var canDelete = await _groupRepository.IsUserAdminInGroup(userId.Value, contest.GroupId, cancellationToken);
            if (canDelete == false)
            {
                return Result<bool>.Fail("User is not admin", ErrorCode.Forbidden);
            }

            var teams = await _teamRepository.GetByContestIdAsync(request.contestId, cancellationToken);

            var affectedUserIds = teams
                                    .SelectMany(t => t.TeamMembers.Select(m => m.UserId))
                                    .Distinct()
                                    .ToList();

            // 4. Xoá dữ liệu liên quan (cascade)
            await _contestRepository.DeleteIndividualContestActivityByContestId(request.contestId, cancellationToken);
            await _teamMemberActivityRepository.DeleteByContestIdAsync(request.contestId, cancellationToken);
            await _teamRepository.DeleteByContestIdAsync(request.contestId, cancellationToken);

            var deleted = await _contestRepository.DeleteAsync(contest, cancellationToken);
            if (!deleted) 
            {                 
                return Result<bool>.Fail("Failed to delete contest", ErrorCode.InternalError);
            }
            // 5. Cleanup cache
            if (affectedUserIds.Count > 0)
            {
                await _userTeamsCacheService.CleanupCacheForUsersAsync(affectedUserIds, cancellationToken);
            }
            return Result<bool>.Success(true);

        }
    }
}
