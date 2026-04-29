using Uracle.Application.Abstractions.Services;
using Uracle.Application.DTOs.StravasDto;
using Uracle.Domain.Enums;
using Uracle.Domain.Models.Contests;

namespace Uracle.Application.Commands.StravaCommand
{
    public record SyncStravaActivityCreateCommand(string EventId, long ActivityId, long OwnerId)
        : ICommand<Result<bool>>;

    public class SyncStravaActivityCreateCommandHandler
        : ICommandHandler<SyncStravaActivityCreateCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IWorkoutActivityRepository _workoutActivityRepo;
        private readonly IStravaActivityApiService _stravaActivityApi;
        private readonly IContestRepository _contestRepository;
        private readonly ITeamMemberActivityRepository _teamMemberActivityRepo;
        private readonly ILogger<SyncStravaActivityCreateCommandHandler> _logger;

        public SyncStravaActivityCreateCommandHandler(
            IUserRepository userRepository,
            IWorkoutActivityRepository workoutActivityRepo,
            IStravaActivityApiService stravaActivityApi,
            IContestRepository contestRepository,
            ITeamMemberActivityRepository teamMemberActivityRepo,
            ILogger<SyncStravaActivityCreateCommandHandler> logger)
        {
            _userRepository = userRepository;
            _workoutActivityRepo = workoutActivityRepo;
            _stravaActivityApi = stravaActivityApi;
            _contestRepository = contestRepository;
            _teamMemberActivityRepo = teamMemberActivityRepo;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(SyncStravaActivityCreateCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByStravaIdAsync(request.OwnerId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("No user found for Strava owner_id {OwnerId}", request.OwnerId);
                return Result<bool>.Success(false);
            }

            // Dedup: already synced?
            var existing = await _workoutActivityRepo.GetByStravaActivityIdAsync(request.ActivityId, cancellationToken);
            if (existing != null)
            {
                _logger.LogInformation("Activity {ActivityId} already synced, skipping", request.ActivityId);
                return Result<bool>.Success(true);
            }

            var accessToken = await _stravaActivityApi.EnsureValidAccessTokenAsync(user, cancellationToken);
            if (accessToken == null)
                return Result<bool>.Fail("Unable to refresh Strava access token", ErrorCode.Unauthorized);

            var stravaActivity = await _stravaActivityApi.GetActivityAsync(request.ActivityId, accessToken, cancellationToken);
            if (stravaActivity == null)
                return Result<bool>.Fail($"Activity {request.ActivityId} not found on Strava", ErrorCode.NotFound);

            var pace = ComputePace(stravaActivity.Distance, stravaActivity.MovingTime);
            var workoutActivity = new WorkoutActivity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                StravaUserId = (int)user.StravaId!.Value,
                Distance = stravaActivity.Distance,
                MovingTime = stravaActivity.MovingTime,
                WorkoutType = stravaActivity.ResolvedType,
                Pace = pace,
                StartDate = stravaActivity.StartDate,
                StravaActivityId = stravaActivity.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _workoutActivityRepo.CreateAsync(workoutActivity, cancellationToken);
            await PropagateToContestsAsync(user, workoutActivity, stravaActivity, pace, cancellationToken);

            return Result<bool>.Success(true);
        }

        private async Task PropagateToContestsAsync(User user, WorkoutActivity workoutActivity,
            StravaActivityResponseDto stravaActivity, double? pace, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var activeContests = await _contestRepository.GetActiveContestsForUserAsync(user.Id, now, ct);

            foreach (var (contest, teamId) in activeContests)
            {
                // Filter by activity type, pace range, minimum distance, and contest window
                if (!IsActivityTypeMatch(contest.ActivityType, workoutActivity.WorkoutType)) continue;
                if (contest.MinPace.HasValue && pace.HasValue && pace.Value < contest.MinPace.Value) continue;
                if (contest.MaxPace.HasValue && pace.HasValue && pace.Value > contest.MaxPace.Value) continue;
                if (contest.MinDistance.HasValue && stravaActivity.Distance / 1000.0 < contest.MinDistance.Value) continue;
                if (stravaActivity.StartDate < contest.StartAt || stravaActivity.StartDate > contest.EndAt) continue;

                if (contest.ContestType == ContestType.Individual)
                {
                    await _contestRepository.AddIndividualContestActivityAsync(new IndividualContestActivity
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        ContestId = contest.Id,
                        StravaUserId = (int)user.StravaId!.Value,
                        Distance = stravaActivity.Distance,
                        MovingTime = stravaActivity.MovingTime,
                        WorkoutType = workoutActivity.WorkoutType,
                        Pace = pace,
                        StartDate = stravaActivity.StartDate,
                        WorkoutActivityId = workoutActivity.Id,
                        StravaActivityId = stravaActivity.Id
                    }, ct);
                }
                else if (contest.ContestType == ContestType.Team && teamId != null)
                {
                    await _teamMemberActivityRepo.CreateAsync(new TeamMemberActivity
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        TeamId = teamId,
                        ContestId = contest.Id,
                        StravaUserId = (int)user.StravaId!.Value,
                        Distance = stravaActivity.Distance,
                        MovingTime = stravaActivity.MovingTime,
                        WorkoutType = workoutActivity.WorkoutType,
                        Pace = pace,
                        StartDate = stravaActivity.StartDate,
                        WorkoutActivityId = workoutActivity.Id,
                        StravaActivityId = stravaActivity.Id
                    }, ct);
                }
            }
        }

        private static double? ComputePace(double distanceMeters, int movingTimeSec)
        {
            if (distanceMeters <= 0 || movingTimeSec <= 0) return null;
            return (movingTimeSec / 60.0) / (distanceMeters / 1000.0); // min/km
        }

        private static bool IsActivityTypeMatch(ActivityType contestType, string workoutType) =>
            contestType == ActivityType.All ||
            (contestType == ActivityType.Run && workoutType.Contains("Run", StringComparison.OrdinalIgnoreCase)) ||
            (contestType == ActivityType.Ride && workoutType.Contains("Ride", StringComparison.OrdinalIgnoreCase)) ||
            (contestType == ActivityType.Swim && workoutType.Contains("Swim", StringComparison.OrdinalIgnoreCase));
    }
}
