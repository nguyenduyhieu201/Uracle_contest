namespace Uracle.Application.Queries.GroupsQuery
{
    public record GetUserRoleInGroupQuery(string GroupId, string Token) : IQuery<Result<string?>>;

    public class GetUserRoleInGroupQueryHandler : IQueryHandler<GetUserRoleInGroupQuery, Result<string?>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public GetUserRoleInGroupQueryHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<string?>> Handle(GetUserRoleInGroupQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<string?>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var role = await _groupRepository.GetUserRoleAsync(request.GroupId, userIdResult.Value!, cancellationToken);
            return Result<string?>.Success(role?.ToString());
        }
    }
}
