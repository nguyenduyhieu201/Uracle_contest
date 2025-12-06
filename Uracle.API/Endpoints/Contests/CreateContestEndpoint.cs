
using Microsoft.AspNetCore.Http;

namespace Uracle.API.Endpoints.Contests
{
    public class CreateContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/contest/create", async (ISender sender, HttpContext context) =>
            {
                var token = context.Request.Cookies["AccessToken"];

                // Placeholder for creating a contest
                return Results.Ok(new { Message = "CreateContestEndpoint is not yet implemented." });
            }).RequireAuthorization();  
        }
    }
}
