using Uracle.Application.DTOs.StravasDto;

namespace Uracle.Application.Abstractions.Services
{
    public interface IStravaActivityApiService
    {
        /// <summary>Fetch a single activity from Strava API.</summary>
        Task<StravaActivityResponseDto?> GetActivityAsync(long activityId, string accessToken, CancellationToken ct = default);

        /// <summary>
        /// Refresh an expired Strava access token and persist the new tokens.
        /// Returns the valid access token, or null on failure.
        /// </summary>
        Task<string?> EnsureValidAccessTokenAsync(User user, CancellationToken ct = default);
    }
}
