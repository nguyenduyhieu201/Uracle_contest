using Uracle.Application.Queries.UsersQuery;

namespace Uracle.API.Endpoints.Stravas
{
    public class StravaCallBackEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/strava/callback", async (ISender sender, HttpContext httpContext, IConfiguration cfg) =>
            {
                var code = httpContext.Request.Query["code"].ToString();
                var state = httpContext.Request.Query["state"].ToString();
                var error = httpContext.Request.Query["error"].ToString();

                // 1) Validate state
                var FRONTEND_URL = cfg["Frontend:Url"] ?? "http://localhost:3000";
                var result = await sender.Send(new StravaCallbackQuery(code, state, error));
                // This is a placeholder for the Strava callback handling logic.
                // You would typically extract query parameters, exchange authorization code for tokens, etc.
                return Results.Redirect(FRONTEND_URL + (result.IsSuccess ? result.Value.url : result.Message));
            }).RequireAuthorization();
        }
    }
}
