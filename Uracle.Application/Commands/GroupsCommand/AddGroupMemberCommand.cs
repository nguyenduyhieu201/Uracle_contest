using Uracle.Domain.Enums;

namespace Uracle.Application.Commands.GroupsCommand
{
    public record AddGroupMemberCommand(string GroupId, string Token, string TargetUserId, string Role = "member") : ICommand<Result<string>>;

    public class AddGroupMemberCommandHandler : ICommandHandler<AddGroupMemberCommand, Result<string>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public AddGroupMemberCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<string>> Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<string>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var isAdmin = await _groupRepository.IsUserAdminInGroup(userIdResult.Value!, request.GroupId, cancellationToken);
            if (!isAdmin)
                return Result<string>.Fail("Only admins can add members", ErrorCode.Forbidden);

            var role = request.Role.ToLower() == "admin" ? UserRole.admin : UserRole.member;
            var member = await _groupRepository.AddGroupMemberAsync(request.GroupId, request.TargetUserId, role, cancellationToken);
            return Result<string>.Success(member.Id);
        }
    }
}
