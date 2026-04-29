// File: Uracle.Infrastructure/Services/StravaActivityApiService.cs
using System.Net.Http.Headers;
using System.Text.Json;
using Uracle.Application.Abstractions.Services;
using Uracle.Application.DTOs.StravasDto;

namespace Uracle.Infrastructure.Services;

/// <summary>
/// Wraps Strava REST API calls for activity sync and token refresh.
/// </summary>
public class StravaActivityApiService : IStravaActivityApiService
{
    private const string StravaApiBase = "https://www.strava.com/api/v3";
    private const string StravaTokenUrl = "https://www.strava.com/oauth/token";

    private readonly HttpClient _http;
    private readonly IOptions<StravaOptions> _stravaOptions;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<StravaActivityApiService> _logger;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public StravaActivityApiService(
        HttpClient http,
        IOptions<StravaOptions> stravaOptions,
        IUserRepository userRepository,
        ILogger<StravaActivityApiService> logger)
    {
        _http = http;
        _stravaOptions = stravaOptions;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<StravaActivityResponseDto?> GetActivityAsync(long activityId, string accessToken, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{StravaApiBase}/activities/{activityId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            using var response = await _http.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Strava GET activity {Id} returned {Status}", activityId, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<StravaActivityResponseDto>(json, _jsonOpts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Strava activity {Id}", activityId);
            return null;
        }
    }

    public async Task<string?> EnsureValidAccessTokenAsync(User user, CancellationToken ct = default)
    {
        var nowUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Token still valid — return as-is
        if (user.ExpiresAt.HasValue && user.ExpiresAt.Value > nowUnix)
            return user.AccessToken;

        if (string.IsNullOrEmpty(user.RefreshToken))
        {
            _logger.LogWarning("User {UserId} has no Strava refresh token", user.Id);
            return null;
        }

        var opts = _stravaOptions.Value;
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["client_id"] = opts.ClientId,
            ["client_secret"] = opts.ClientSecret,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = user.RefreshToken
        });

        try
        {
            using var response = await _http.PostAsync(StravaTokenUrl, form, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Strava token refresh failed for user {UserId}: {Status}", user.Id, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var tokenResponse = JsonSerializer.Deserialize<StravaTokenResponse>(json, _jsonOpts);
            if (tokenResponse?.AccessToken == null) return null;

            await _userRepository.UpdateUserTokensAsync(user.Id, tokenResponse, ct);
            return tokenResponse.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing Strava token for user {UserId}", user.Id);
            return null;
        }
    }
}
