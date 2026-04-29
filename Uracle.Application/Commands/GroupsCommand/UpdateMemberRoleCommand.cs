using Uracle.Domain.Enums;

namespace Uracle.Application.Commands.GroupsCommand
{
    public record UpdateMemberRoleCommand(string GroupId, string TargetUserId, string Role, string Token) : ICommand<Result<bool>>;

    public class UpdateMemberRoleCommandHandler : ICommandHandler<UpdateMemberRoleCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public UpdateMemberRoleCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<bool>> Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<bool>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var isAdmin = await _groupRepository.IsUserAdminInGroup(userIdResult.Value!, request.GroupId, cancellationToken);
            if (!isAdmin)
                return Result<bool>.Fail("Only admins can update member roles", ErrorCode.Forbidden);

            if (!Enum.TryParse<UserRole>(request.Role.ToLower(), out var role))
                return Result<bool>.Fail("Invalid role. Must be 'admin' or 'member'", ErrorCode.BadRequest);

            var success = await _groupRepository.UpdateMemberRoleAsync(request.GroupId, request.TargetUserId, role, cancellationToken);
            if (!success)
                return Result<bool>.Fail("Member not found", ErrorCode.NotFound);

            return Result<bool>.Success(true);
        }
    }
}
