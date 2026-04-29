using Uracle.Application.DTOs.GroupsDto;

namespace Uracle.Application.Queries.GroupsQuery
{
    public record GetPendingJoinRequestsQuery(string GroupId, string Token) : IQuery<Result<List<JoinRequestDto>>>;

    public class GetPendingJoinRequestsQueryHandler : IQueryHandler<GetPendingJoinRequestsQuery, Result<List<JoinRequestDto>>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public GetPendingJoinRequestsQueryHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<List<JoinRequestDto>>> Handle(GetPendingJoinRequestsQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<List<JoinRequestDto>>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var isAdmin = await _groupRepository.IsUserAdminInGroup(userIdResult.Value!, request.GroupId, cancellationToken);
            if (!isAdmin)
                return Result<List<JoinRequestDto>>.Fail("Only admins can view join requests", ErrorCode.Forbidden);

            var requests = await _groupRepository.GetPendingJoinRequestsAsync(request.GroupId, cancellationToken);
            var dtos = requests.Select(r => new JoinRequestDto
            {
                UserId = r.UserId,
                Username = r.User?.Username ?? r.UserId,
                DisplayName = r.User?.DisplayName,
                RequestedAt = r.RequestedAt
            }).ToList();

            return Result<List<JoinRequestDto>>.Success(dtos);
        }
    }
}
