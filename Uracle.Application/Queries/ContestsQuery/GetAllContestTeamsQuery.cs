namespace Uracle.Application.Queries.ContestsQuery
{
    public record GetContestTeamsQuery(string ContestId)
        : IQuery<Result<List<ContestTeamDto>>>;
    public record ContestTeamDto(
        string Id,
        string Name,
        int Number
    );
    public sealed class GetContestTeamsQueryHandler
    : IQueryHandler<GetContestTeamsQuery, Result<List<ContestTeamDto>>>
    {
        private readonly ITeamRepository _teamRepository;
        public GetContestTeamsQueryHandler(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        public async Task<Result<List<ContestTeamDto>>> Handle(
            GetContestTeamsQuery request,
            CancellationToken cancellationToken)
        {
            
            var teams = await _teamRepository
                .GetByContestIdAsync(request.ContestId, cancellationToken);
            // Không tìm thấy team nào vẫn coi là success, chỉ trả về list rỗng
            if (teams.Count == 0)
            {
                return Result<List<ContestTeamDto>>.Success(new List<ContestTeamDto>());
            }
            var teamDtos = teams
                .Select(t => new ContestTeamDto(
                    t.Id,
                    t.Name,
                    t.NumberOfMembers))
                .ToList();
            return Result<List<ContestTeamDto>>.Success(teamDtos);
        }
    }
}
