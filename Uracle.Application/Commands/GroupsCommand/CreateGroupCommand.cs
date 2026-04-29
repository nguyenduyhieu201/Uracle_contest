using Uracle.Domain.Enums;

namespace Uracle.Application.Commands.GroupsCommand
{
    public record CreateGroupCommand(string name, string description, bool isPrivate, string? userName) : ICommand<Result<string>>;
    public record CreateGroupResponse(string groupId, string name, string description);

    public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
    {
        public CreateGroupCommandValidator()
        {
            RuleFor(x => x.name).NotEmpty().WithMessage("Group name is required");
            RuleFor(x => x.description).NotEmpty().WithMessage("Group description is required");
        }
    }

    public record CreateGroupCommandHandler: ICommandHandler<CreateGroupCommand, Result<string>>
    {
        private readonly IGroupRepository _groupRepository;
        public CreateGroupCommandHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<Result<string>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var newGroup = new Group
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.name,
                Description = request.description,
                CreatedAt = DateTime.UtcNow,
                IsPrivate = request.isPrivate,
                CreatedBy = request.userName, 
                MemberCount = 1
            };
            await _groupRepository.AddAsync(newGroup, cancellationToken);

            if (!string.IsNullOrEmpty(request.userName))
                await _groupRepository.AddGroupMemberAsync(newGroup.Id, request.userName, UserRole.admin, cancellationToken);

            return Result<string>.Success(newGroup.Id);
        }
    }
}
