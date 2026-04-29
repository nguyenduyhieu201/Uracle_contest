namespace Uracle.Application.Queries.UsersQuery
{
    public record GetUserTeamsQuery(string? TargetUserId, string Token) : IQuery<Result<List<Team>>>;

    public class GetUserTeamsQueryHandler : IQueryHandler<GetUserTeamsQuery, Result<List<Team>>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IJwtService _jwtService;

        public GetUserTeamsQueryHandler(ITeamRepository teamRepository, IJwtService jwtService)
        {
            _teamRepository = teamRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<List<Team>>> Handle(GetUserTeamsQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<List<Team>>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var targetUserId = request.TargetUserId ?? userIdResult.Value!;
            var teams = await _teamRepository.GetByUserIdAsync(targetUserId, cancellationToken);
            return Result<List<Team>>.Success(teams);
        }
    }
}
