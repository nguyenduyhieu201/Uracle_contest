using Uracle.Application.DTOs.GroupsDto;

namespace Uracle.Application.Queries.GroupsQuery
{
    public record GetGroupMembersQuery(string GroupId, string Token, int Page = 1, int Limit = 20) : IQuery<Result<List<GroupMemberDto>>>;

    public class GetGroupMembersQueryHandler : IQueryHandler<GetGroupMembersQuery, Result<List<GroupMemberDto>>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public GetGroupMembersQueryHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<List<GroupMemberDto>>> Handle(GetGroupMembersQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<List<GroupMemberDto>>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
            if (group == null)
                return Result<List<GroupMemberDto>>.Fail("Group not found", ErrorCode.NotFound);

            var userId = userIdResult.Value!;
            if (group.IsPrivate)
            {
                var isMember = await _groupRepository.IsMemberAsync(request.GroupId, userId, cancellationToken);
                if (!isMember)
                    return Result<List<GroupMemberDto>>.Fail("Access denied to view group members", ErrorCode.Forbidden);
            }

            var members = await _groupRepository.GetGroupMembersAsync(request.GroupId, request.Page, request.Limit, cancellationToken);
            var dtos = members.Select(m => new GroupMemberDto
            {
                UserId = m.UserId,
                Username = m.User?.Username ?? m.UserId,
                DisplayName = m.User?.DisplayName,
                Role = m.Role.ToString(),
                JoinedAt = m.JoinedAt
            }).ToList();

            return Result<List<GroupMemberDto>>.Success(dtos);
        }
    }
}
