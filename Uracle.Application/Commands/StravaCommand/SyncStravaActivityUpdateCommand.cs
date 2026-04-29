using Uracle.Application.Abstractions.Services;

namespace Uracle.Application.Commands.StravaCommand
{
    public record SyncStravaActivityUpdateCommand(string EventId, long ActivityId, long OwnerId)
        : ICommand<Result<bool>>;

    public class SyncStravaActivityUpdateCommandHandler
        : ICommandHandler<SyncStravaActivityUpdateCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IWorkoutActivityRepository _workoutActivityRepo;
        private readonly IStravaActivityApiService _stravaActivityApi;
        private readonly IContestRepository _contestRepository;
        private readonly ITeamMemberActivityRepository _teamMemberActivityRepo;
        private readonly ILogger<SyncStravaActivityUpdateCommandHandler> _logger;

        public SyncStravaActivityUpdateCommandHandler(
            IUserRepository userRepository,
            IWorkoutActivityRepository workoutActivityRepo,
            IStravaActivityApiService stravaActivityApi,
            IContestRepository contestRepository,
            ITeamMemberActivityRepository teamMemberActivityRepo,
            ILogger<SyncStravaActivityUpdateCommandHandler> logger)
        {
            _userRepository = userRepository;
            _workoutActivityRepo = workoutActivityRepo;
            _stravaActivityApi = stravaActivityApi;
            _contestRepository = contestRepository;
            _teamMemberActivityRepo = teamMemberActivityRepo;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(SyncStravaActivityUpdateCommand request, CancellationToken cancellationToken)
        {
            var workoutActivity = await _workoutActivityRepo.GetByStravaActivityIdAsync(request.ActivityId, cancellationToken);
            if (workoutActivity == null)
            {
                _logger.LogWarning("Activity {ActivityId} not found locally for update, skipping", request.ActivityId);
                return Result<bool>.Success(false);
            }

            var user = await _userRepository.GetByStravaIdAsync(request.OwnerId, cancellationToken);
            if (user == null)
                return Result<bool>.Fail("User not found", ErrorCode.NotFound);

            var accessToken = await _stravaActivityApi.EnsureValidAccessTokenAsync(user, cancellationToken);
            if (accessToken == null)
                return Result<bool>.Fail("Unable to refresh Strava access token", ErrorCode.Unauthorized);

            var stravaActivity = await _stravaActivityApi.GetActivityAsync(request.ActivityId, accessToken, cancellationToken);
            if (stravaActivity == null)
                return Result<bool>.Fail($"Activity {request.ActivityId} not found on Strava", ErrorCode.NotFound);

            var pace = stravaActivity.Distance > 0 && stravaActivity.MovingTime > 0
                ? (stravaActivity.MovingTime / 60.0) / (stravaActivity.Distance / 1000.0)
                : (double?)null;

            // Update WorkoutActivity
            workoutActivity.Distance = stravaActivity.Distance;
            workoutActivity.MovingTime = stravaActivity.MovingTime;
            workoutActivity.WorkoutType = stravaActivity.ResolvedType;
            workoutActivity.Pace = pace;
            workoutActivity.StartDate = stravaActivity.StartDate;
            workoutActivity.UpdatedAt = DateTime.UtcNow;

            await _workoutActivityRepo.UpdateAsync(workoutActivity, cancellationToken);

            // Propagate updates to all related contest activities
            await _contestRepository.UpdateIndividualActivitiesByWorkoutIdAsync(
                workoutActivity.Id, stravaActivity.Distance, stravaActivity.MovingTime,
                workoutActivity.WorkoutType, pace, cancellationToken);

            await _teamMemberActivityRepo.UpdateByWorkoutActivityIdAsync(
                workoutActivity.Id, stravaActivity.Distance, stravaActivity.MovingTime,
                workoutActivity.WorkoutType, pace, cancellationToken);

            _logger.LogInformation("Updated activity {ActivityId} and its contest records", request.ActivityId);
            return Result<bool>.Success(true);
        }
    }
}
