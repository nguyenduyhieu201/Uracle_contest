namespace Uracle.Application.Queries.ContestsQuery
{
    public sealed record GetIndividualContestActivitiesQuery(
        string ContestId,
        string UserId
    ) : IQuery<Result<List<IndividualContestActivityDto>>>;

    public sealed class GetIndividualContestActivitiesQueryHandler
    : IQueryHandler<GetIndividualContestActivitiesQuery, Result<List<IndividualContestActivityDto>>>
    {
        private readonly IContestRepository _contestRepository;
        public GetIndividualContestActivitiesQueryHandler(
            IContestRepository contestRepository)
        {
            _contestRepository = contestRepository;
        }
        public async Task<Result<List<IndividualContestActivityDto>>> Handle(
            GetIndividualContestActivitiesQuery request,
            CancellationToken cancellationToken)
        {
            var activities = await _contestRepository
                .GetByContestAndUserAsync(request.ContestId, request.UserId, cancellationToken);
            var dtos = activities
                        .Select(a => new IndividualContestActivityDto(
                            a.Id,
                            a.ContestId,
                            a.UserId,
                            a.StartDate,
                            a.Distance,
                            a.Pace))
                        .ToList();
            return Result<List<IndividualContestActivityDto>>.Success(dtos);
        }
    }
}
