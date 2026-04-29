using Uracle.Application.DTOs.GroupsDto;

namespace Uracle.Application.Queries.GroupsQuery
{
    public record GetGroupByIdQuery(string GroupId, string Token) : IQuery<Result<GroupDetailDto>>;

    public class GetGroupByIdQueryHandler : IQueryHandler<GetGroupByIdQuery, Result<GroupDetailDto>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public GetGroupByIdQueryHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<GroupDetailDto>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<GroupDetailDto>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
            if (group == null)
                return Result<GroupDetailDto>.Fail("Group not found", ErrorCode.NotFound);

            var userId = userIdResult.Value!;
            if (group.IsPrivate)
            {
                var isMember = await _groupRepository.IsMemberAsync(request.GroupId, userId, cancellationToken);
                if (!isMember)
                    return Result<GroupDetailDto>.Fail("Access denied to private group", ErrorCode.Forbidden);
            }

            return Result<GroupDetailDto>.Success(new GroupDetailDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                IsPrivate = group.IsPrivate,
                MemberCount = group.MemberCount,
                CreatedAt = group.CreatedAt
            });
        }
    }
}
