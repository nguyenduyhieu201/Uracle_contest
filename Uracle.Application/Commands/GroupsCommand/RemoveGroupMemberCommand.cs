namespace Uracle.Application.Commands.GroupsCommand
{
    public record RemoveGroupMemberCommand(string GroupId, string TargetUserId, string Token) : ICommand<Result<bool>>;

    public class RemoveGroupMemberCommandHandler : ICommandHandler<RemoveGroupMemberCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public RemoveGroupMemberCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<bool>> Handle(RemoveGroupMemberCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<bool>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var isAdmin = await _groupRepository.IsUserAdminInGroup(userIdResult.Value!, request.GroupId, cancellationToken);
            if (!isAdmin)
                return Result<bool>.Fail("Only admins can remove members", ErrorCode.Forbidden);

            var success = await _groupRepository.RemoveMemberAsync(request.GroupId, request.TargetUserId, cancellationToken);
            if (!success)
                return Result<bool>.Fail("Member not found", ErrorCode.NotFound);

            return Result<bool>.Success(true);
        }
    }
}
