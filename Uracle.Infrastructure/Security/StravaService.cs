using Azure.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.Queries.UsersQuery;
using Uracle.Infrastructure.Options;

namespace Uracle.Infrastructure.Security
{
    public class StravaService : IStravaService
    {
        private readonly IOptions<StravaOptions> _opts;
        private readonly IJWTService _jWTService;
        private const string STRAVA_AUTHORIZE_URL = "https://www.strava.com/oauth/authorize";
        public StravaService(IOptions<StravaOptions> opts, IJWTService jWTService)
        {
            _opts = opts;
            _jWTService = jWTService;
        }

        public async Task<Result<string>> HandleAuthorizeUrl(string token)
        {
            var o = _opts.Value;
            if (string.IsNullOrWhiteSpace(o.ClientId) || string.IsNullOrWhiteSpace(o.RedirectUri))
            {
                return Result<string>.Fail("Strava options not configured");
            }
            var validateResult = await _jWTService.ValidateUserAsync(token);
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
    }
}
