namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IUserTeamsCacheService
    {
        Task CleanupCacheForUserAsync(
            string userId,
            CancellationToken cancellationToken = default);

        Task CleanupCacheForUsersAsync(
            List<string> userIds,
            CancellationToken cancellationToken = default);

        Task CacheUserTeamsAsync(string userId, CancellationToken cancellationToken = default);

    }

}
