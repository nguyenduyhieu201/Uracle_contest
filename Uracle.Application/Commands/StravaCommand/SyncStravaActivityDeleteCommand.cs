namespace Uracle.Application.Commands.StravaCommand
{
    public record SyncStravaActivityDeleteCommand(string EventId, long ActivityId)
        : ICommand<Result<bool>>;

    public class SyncStravaActivityDeleteCommandHandler
        : ICommandHandler<SyncStravaActivityDeleteCommand, Result<bool>>
    {
        private readonly IWorkoutActivityRepository _workoutActivityRepo;
        private readonly IContestRepository _contestRepository;
        private readonly ITeamMemberActivityRepository _teamMemberActivityRepo;
        private readonly ILogger<SyncStravaActivityDeleteCommandHandler> _logger;

        public SyncStravaActivityDeleteCommandHandler(
            IWorkoutActivityRepository workoutActivityRepo,
            IContestRepository contestRepository,
            ITeamMemberActivityRepository teamMemberActivityRepo,
            ILogger<SyncStravaActivityDeleteCommandHandler> logger)
        {
            _workoutActivityRepo = workoutActivityRepo;
            _contestRepository = contestRepository;
            _teamMemberActivityRepo = teamMemberActivityRepo;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(SyncStravaActivityDeleteCommand request, CancellationToken cancellationToken)
        {
            var workoutActivity = await _workoutActivityRepo.GetByStravaActivityIdAsync(request.ActivityId, cancellationToken);
            if (workoutActivity == null)
            {
                _logger.LogWarning("Activity {ActivityId} not found locally for delete, skipping", request.ActivityId);
                return Result<bool>.Success(false);
            }

            // Remove contest activity records first (FK constraints)
            await _contestRepository.DeleteIndividualActivitiesByWorkoutIdAsync(workoutActivity.Id, cancellationToken);
            await _teamMemberActivityRepo.DeleteByWorkoutActivityIdAsync(workoutActivity.Id, cancellationToken);

            // Delete the workout activity
            await _workoutActivityRepo.DeleteByStravaActivityIdAsync(request.ActivityId, cancellationToken);

            _logger.LogInformation("Deleted activity {ActivityId} and its contest records", request.ActivityId);
            return Result<bool>.Success(true);
        }
    }
}
