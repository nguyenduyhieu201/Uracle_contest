namespace Uracle.Application.Queries.TeamsQuery
{

    public record GetTeamByIdQuery(string Id) : IQuery<Result<GetTeamByIdResponse>>;
    public record GetTeamByIdResponse(
        string Id,
        string Name,
        string GroupId,
        string ContestId,
        int NumberOfMember,
        List<TeamMemberDto> Members,
        double AveragePace,
        double TotalDistance,
        double TotalTracklog,
        double FastestPace,
        double MaxDistance

    );
    public class GetTeamByIdQueryHandler : IQueryHandler<GetTeamByIdQuery, Result<GetTeamByIdResponse>>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        public GetTeamByIdQueryHandler(ITeamRepository teamRepository, IUserRepository userRepository)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
        }
        public async Task<Result<GetTeamByIdResponse>> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(request.Id, cancellationToken);
            if (team == null)
                return Result<GetTeamByIdResponse>.Fail("Team not found", ErrorCode.NotFound);
            var memberDtos = new List<TeamMemberDto>();
            
            foreach (var member in team.TeamMembers)
            {
                //var user = await _userRepository.GetUserByIdAsync(member.UserId, cancellationToken);
                var user = member.User;
                memberDtos.Add(new TeamMemberDto(
                    member.UserId,
                    member.JoinedAt,
                    user?.StravaProfile?.Firstname != null && user?.StravaProfile?.Lastname != null
                        ? $"{user.StravaProfile.Firstname} {user.StravaProfile.Lastname}"
                        : user?.Username ?? member.UserId.ToString(),
                    user?.Username ?? member.UserId.ToString(),
                    user?.StravaProfile != null ? new StravaProfileDto(
                        user.StravaProfile.Firstname,
                        user.StravaProfile.Lastname
                    ) : null
                ));
            }
            var response = new GetTeamByIdResponse(
                team.Id,
                team.Name,
                team.GroupId,
                team.ContestId,
                team.NumberOfMembers,
                memberDtos,
                team.AveragePace,
                team.TotalDistance,
                team.TotalTrackLog,
                team.FastestPace,
                team.MaxDistance

            );
            return Result<GetTeamByIdResponse>.Success(response);
        }
    }
}
