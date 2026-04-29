namespace Uracle.Application.Commands.GroupsCommand
{
    public record DeleteGroupCommand(string GroupId, string Token) : ICommand<Result<bool>>;

    public class DeleteGroupCommandHandler : ICommandHandler<DeleteGroupCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public DeleteGroupCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<bool>> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<bool>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var isAdmin = await _groupRepository.IsUserAdminInGroup(userIdResult.Value!, request.GroupId, cancellationToken);
            if (!isAdmin)
                return Result<bool>.Fail("Only admins can delete groups", ErrorCode.Forbidden);

            var success = await _groupRepository.DeleteAsync(request.GroupId, cancellationToken);
            if (!success)
                return Result<bool>.Fail("Group not found", ErrorCode.NotFound);

            return Result<bool>.Success(true);
        }
    }
}
