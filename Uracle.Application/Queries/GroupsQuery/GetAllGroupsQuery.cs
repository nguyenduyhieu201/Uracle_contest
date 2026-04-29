using Uracle.Application.DTOs.GroupsDto;

namespace Uracle.Application.Queries.GroupsQuery
{
    public record GetAllGroupsQuery(string Token) : IQuery<Result<GroupsListResponse>>;

    public class GetAllGroupsQueryHandler : IQueryHandler<GetAllGroupsQuery, Result<GroupsListResponse>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public GetAllGroupsQueryHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<GroupsListResponse>> Handle(GetAllGroupsQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<GroupsListResponse>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var userId = userIdResult.Value!;
            var allGroups = await _groupRepository.GetAllAsync();
            var userMemberships = await _groupRepository.FindByUserIdAsync(userId, cancellationToken);
            var userGroupIds = userMemberships.Select(m => m.GroupId).ToHashSet();

            var myGroups = new List<GroupResponse>();
            var otherGroups = new List<GroupResponse>();

            foreach (var group in allGroups)
            {
                var response = new GroupResponse(
                    group.Id,
                    group.Name,
                    group.Description,
                    group.IsPrivate,
                    group.MemberCount);

                if (userGroupIds.Contains(group.Id))
                    myGroups.Add(response);
                else
                    otherGroups.Add(response);
            }

            return Result<GroupsListResponse>.Success(new GroupsListResponse(myGroups, otherGroups));
        }
    }
}
