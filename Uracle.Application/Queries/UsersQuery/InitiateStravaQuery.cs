

using Microsoft.Extensions.Options;
namespace Uracle.Application.Queries.UsersQuery
{
    public record InitiateStravaQuery(string token) : IQuery<Result<InitiateStravaResponse>>;
    public record InitiateStravaResponse(string AuthorizationUrl);

    public class InitiateStravaQueryHandler
                : IQueryHandler<InitiateStravaQuery, Result<InitiateStravaResponse>>
    {
        private IStravaService _stravaService;
        public InitiateStravaQueryHandler(IStravaService stravaService)
        {
            _stravaService = stravaService;
        }
        public async Task<Result<InitiateStravaResponse>> Handle(InitiateStravaQuery request, CancellationToken cancellationToken)
        {
            var result = await _stravaService.HandleAuthorizeUrl(request.token);
            if (result.IsFail)
                return Result<InitiateStravaResponse>.Fail(result.Message ?? "Failed to build Strava authorize url");
            var url = result.Value!;
            return Result<InitiateStravaResponse>.Success(new InitiateStravaResponse(url));
        }
    }
}
