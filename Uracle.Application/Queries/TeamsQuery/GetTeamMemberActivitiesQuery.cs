namespace Uracle.Application.Queries.TeamsQuery
{
    public record GetTeamMemberActivitiesQuery (string teamId, string userId) : IQuery<Result<List<TeamMemberActivityDto>>>;
    public class GetTeamMemberActivitiesQueryHandler : IQueryHandler<GetTeamMemberActivitiesQuery, Result<List<TeamMemberActivityDto>>>
    {
        private readonly ITeamMemberActivityRepository _teamMemberActivityRepository;
        public GetTeamMemberActivitiesQueryHandler(
            ITeamMemberActivityRepository teamMemberActivityRepository)
        {
            _teamMemberActivityRepository = teamMemberActivityRepository;
        }
        public async Task<Result<List<TeamMemberActivityDto>>> Handle(
            GetTeamMemberActivitiesQuery request,
            CancellationToken cancellationToken)
        {
            // Single repository call - this is the core logic
            var activities = await _teamMemberActivityRepository.FindByTeamAndUserAsync(
                request.teamId,
                request.userId,
                cancellationToken);
            // Simple mapping to DTOs
            var activityDtos = activities.Select(MapToDto).ToList();

            return Result<List<TeamMemberActivityDto>>.Success(activityDtos);
        }
        private static TeamMemberActivityDto MapToDto(TeamMemberActivity activity)
        {
            return new TeamMemberActivityDto
            {
                Id = activity.Id,
                TeamId = activity.TeamId,
                UserId = activity.UserId,
                ContestId = activity.ContestId,
                Distance = activity.Distance,
                Pace = activity.Pace.Value,
                StartDate = activity.StartDate,

                ActivityType = activity.WorkoutType,
                StravaActivityId = activity.StravaActivityId.Value,

            };
        }
    }
}