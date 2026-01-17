namespace Uracle.Application.Queries.GroupsQuery
{
    public record GetGroupContestsQuery(string GroupId) : IQuery<Result<List<ContestDto>>>;
    public class GetGroupContestsQueryHandler : IQueryHandler<GetGroupContestsQuery, Result<List<ContestDto>>>
    {
        private readonly IContestRepository _contestRepository;
        public GetGroupContestsQueryHandler(IContestRepository contestRepository)
        {
            _contestRepository = contestRepository;
        }
        public async Task<Result<List<ContestDto>>> Handle(GetGroupContestsQuery request, CancellationToken cancellationToken)
        {
            var contests = await _contestRepository
                .GetByGroupIdAsync(request.GroupId, cancellationToken);
            var dtos = contests.Select(c => new ContestDto
            {
                Id = c.Id,
                GroupId = c.GroupId,
                Name = c.Name,
                StartAt = c.StartAt,
                EndAt = c.EndAt,
                ContestType = c.ContestType
            }).ToList();
            return Result<List<ContestDto>>.Success(dtos);
        }
    }
}
