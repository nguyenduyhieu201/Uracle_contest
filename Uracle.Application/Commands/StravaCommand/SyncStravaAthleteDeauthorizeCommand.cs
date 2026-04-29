namespace Uracle.Application.Commands.StravaCommand
{
    public record SyncStravaAthleteDeauthorizeCommand(string EventId, long OwnerId)
        : ICommand<Result<bool>>;

    public class SyncStravaAthleteDeauthorizeCommandHandler
        : ICommandHandler<SyncStravaAthleteDeauthorizeCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SyncStravaAthleteDeauthorizeCommandHandler> _logger;

        public SyncStravaAthleteDeauthorizeCommandHandler(
            IUserRepository userRepository,
            ILogger<SyncStravaAthleteDeauthorizeCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(SyncStravaAthleteDeauthorizeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByStravaIdAsync(request.OwnerId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("No user found for Strava deauthorize event owner_id {OwnerId}", request.OwnerId);
                return Result<bool>.Success(false);
            }

            await _userRepository.ClearStravaTokensAsync(user.Id, cancellationToken);

            _logger.LogInformation("Cleared Strava tokens for user {UserId} (StravaId={OwnerId})", user.Id, request.OwnerId);
            return Result<bool>.Success(true);
        }
    }
}
