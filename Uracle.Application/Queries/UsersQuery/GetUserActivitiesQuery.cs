using Uracle.Application.DTOs.UsersDto;

namespace Uracle.Application.Queries.UsersQuery
{
    public record GetUserActivitiesQuery(string Token) : IQuery<Result<List<WorkoutActivityDto>>>;

    public class GetUserActivitiesQueryHandler : IQueryHandler<GetUserActivitiesQuery, Result<List<WorkoutActivityDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public GetUserActivitiesQueryHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<List<WorkoutActivityDto>>> Handle(GetUserActivitiesQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<List<WorkoutActivityDto>>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var activities = await _userRepository.GetWorkoutActivitiesByUserIdAsync(userIdResult.Value!, cancellationToken);
            var dtos = activities.Select(a => new WorkoutActivityDto
            {
                Id = a.Id,
                Distance = a.Distance,
                MovingTime = a.MovingTime,
                WorkoutType = a.WorkoutType,
                Pace = a.Pace,
                StartDate = a.StartDate,
                StravaActivityId = a.StravaActivityId
            }).ToList();

            return Result<List<WorkoutActivityDto>>.Success(dtos);
        }
    }
}
