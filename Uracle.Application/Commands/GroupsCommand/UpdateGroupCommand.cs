namespace Uracle.Application.Commands.GroupsCommand
{
    public record UpdateGroupCommand(string GroupId, string Token, string? Name, string? Description, bool? IsPrivate) : ICommand<Result<string>>;

    public class UpdateGroupCommandHandler : ICommandHandler<UpdateGroupCommand, Result<string>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public UpdateGroupCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<string>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<string>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var isAdmin = await _groupRepository.IsUserAdminInGroup(userIdResult.Value!, request.GroupId, cancellationToken);
            if (!isAdmin)
                return Result<string>.Fail("Only admins can update groups", ErrorCode.Forbidden);

            var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
            if (group == null)
                return Result<string>.Fail("Group not found", ErrorCode.NotFound);

            if (request.Name != null) group.Name = request.Name;
            if (request.Description != null) group.Description = request.Description;
            if (request.IsPrivate.HasValue) group.IsPrivate = request.IsPrivate.Value;

            await _groupRepository.UpdateAsync(group, cancellationToken);
            return Result<string>.Success(group.Id);
        }
    }
}
