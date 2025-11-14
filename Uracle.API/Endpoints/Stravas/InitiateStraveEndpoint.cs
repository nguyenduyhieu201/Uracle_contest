
using MediatR;
using Uracle.Application.Abstractions.Security;
using Uracle.Application.Queries.UsersQuery;

namespace Uracle.API.Endpoints.Users
{
    public class InitiateStraveEndpoint : ICarterModule
    {
         
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/connect/strava", async (ISender sender, HttpContext httpContext) => 
            {
                var token = httpContext.Request.Cookies["AccessToken"];
                var result = await sender.Send(new InitiateStravaQuery(token ?? string.Empty));
                if (result.IsFail)
                    return Results.BadRequest(result.Message ?? "Failed to initiate Strava connect");
                var url = result.Value.AuthorizationUrl;
                return Results.Redirect(url);
            }).RequireAuthorization();

        }
    }
}
