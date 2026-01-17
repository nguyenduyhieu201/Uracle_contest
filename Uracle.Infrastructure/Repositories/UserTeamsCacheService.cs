
namespace Uracle.Infrastructure.Repositories
{
    public class UserTeamsCacheService: IUserTeamsCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ITeamRepository _teamRepository;
        public UserTeamsCacheService(IDistributedCache cache, ITeamRepository teamRepository)
        {
            _cache = cache;
            _teamRepository = teamRepository;
        }
        private static string GetUserTeamsCacheKey(string userId)
               => $"user:{userId}:teams";

        public async Task CacheUserTeamsAsync(string userId, CancellationToken cancellationToken = default)
        {
            var teams = await _teamRepository.GetByUserIdAsync(userId, cancellationToken);
            if (teams is null || !teams.Any())
            {
                // Nếu user không còn team nào → xoá cache
                await DeleteUserTeamsCacheAsync(userId, cancellationToken);
                return;
            }
            await CacheUserTeamsAsync(userId, teams, cancellationToken);
        }

        public async Task CleanupCacheForUserAsync(
                        string userId,
                        CancellationToken cancellationToken = default)
        {
            var key = GetUserTeamsCacheKey(userId);
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task CleanupCacheForUsersAsync(
            List<string> userIds,
            CancellationToken cancellationToken = default)
        {
            if (userIds is null || userIds.Count == 0)
                return;

            foreach (var userId in userIds)
            {
                var key = GetUserTeamsCacheKey(userId);
                await _cache.RemoveAsync(key, cancellationToken);
            }
        }

        private async Task CacheUserTeamsAsync(string userId, List<Team> teams, CancellationToken cancellationToken)
        {
            var key = GetUserTeamsCacheKey(userId);
            var teamIds = teams.Select(t => t.Id).ToArray();

            var teamIdsJson = System.Text.Json.JsonSerializer.Serialize(teamIds);
            await _cache.SetStringAsync(key, teamIdsJson, cancellationToken);
        }

        private async Task DeleteUserTeamsCacheAsync(string userId, CancellationToken cancellationToken)
        {
            var key = GetUserTeamsCacheKey(userId);
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
}
