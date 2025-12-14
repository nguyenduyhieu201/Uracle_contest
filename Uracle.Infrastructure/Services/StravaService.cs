using Azure.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Uracle.Application.DTOs;
using Uracle.Application.Queries.UsersQuery;
using Uracle.Infrastructure.Options;
using static System.Net.WebRequestMethods;

namespace Uracle.Infrastructure.Security
{
    public class StravaService : IStravaService
    {
        private readonly IOptions<StravaOptions> _opts;
        private readonly IJWTService _jWTService;
        private string STRAVA_AUTHORIZE_URL = "";
        private string Strava_Token_Url = "";
        private readonly HttpClient _http;
        private readonly IUserRepository _userRepository;
        private readonly IStravaRepository _stravaRepository;
        private IConfiguration _configuration;
        public StravaService(IOptions<StravaOptions> opts, IJWTService jWTService, 
                            HttpClient http, IUserRepository userRepository, 
                            IStravaRepository stravaRepository, IConfiguration configuration)
        {
            _opts = opts;
            _jWTService = jWTService;
            _http = http;
            _userRepository = userRepository;
            _stravaRepository = stravaRepository;
            _configuration = configuration;
            STRAVA_AUTHORIZE_URL = _configuration["Strava:Strava_Authorize_Url"] ?? "";
            Strava_Token_Url = _configuration["Strava:StravaTokenUrl"] ?? "";
        }

        public async Task<Result<string>> HandleAuthorizeUrl(string token)
        {
            var o = _opts.Value;
            if (string.IsNullOrWhiteSpace(o.ClientId) || string.IsNullOrWhiteSpace(o.RedirectUri))
            {
                return Result<string>.Fail("Strava options not configured");
            }
            var validateResult = await _jWTService.ValidateUserAsync(token);
            if (validateResult.IsFail)
            {
                return Result<string>.Fail("Invalid token");
            }
            var query = new Dictionary<string, string?>
            {
                ["client_id"] = o.ClientId,
                ["redirect_uri"] = o.RedirectUri,
                ["response_type"] = "code",
                ["scope"] = o.Scope,
                ["state"] = validateResult.Value
            };

            string authorizeUrl = QueryHelpers.AddQueryString(STRAVA_AUTHORIZE_URL, query);


            return Result<string>.Success(authorizeUrl);
        }

        public async Task<Result<User?>> HandleStravaCallback(string code, string? error, string userId, CancellationToken cancellationToken)
        {
            var o = _opts.Value;

            // 1) Exchange code for tokens
            using var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = o.ClientId,
                ["client_secret"] = o.ClientSecret,
                ["code"] = code,
                ["grant_type"] = "authorization_code",
                ["redirect_uri"] = o.RedirectUri!,
            });
            Uri StravaTokenUrl = new Uri(Strava_Token_Url);
            using var req = new HttpRequestMessage(HttpMethod.Post, StravaTokenUrl)
            {
                Content = form
            };
            req.Headers.Accept.ParseAdd("application/json");
            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                var errBody = await resp.Content.ReadAsStringAsync();
                return Result<User>.Fail($"Strava authorization failed: {error} (userId: {userId})");
            }
            var json = await resp.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<StravaTokenResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            await _userRepository.UpdateUserTokensAsync(userId, tokenResponse, cancellationToken);
            await _stravaRepository.SetStravaProfile(userId, tokenResponse.Athlete, cancellationToken);
            return Result<User>.Success(null);
        }
    }
}
