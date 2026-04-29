namespace Uracle.Application.Queries.GroupsQuery
{
    public record GetJoinStatusQuery(string GroupId, string Token) : IQuery<Result<bool>>;

    public class GetJoinStatusQueryHandler : IQueryHandler<GetJoinStatusQuery, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public GetJoinStatusQueryHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<bool>> Handle(GetJoinStatusQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<bool>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var hasPendingRequest = await _groupRepository.HasPendingJoinRequestAsync(
                request.GroupId, userIdResult.Value!, cancellationToken);

            return Result<bool>.Success(hasPendingRequest);
        }
    }
}
