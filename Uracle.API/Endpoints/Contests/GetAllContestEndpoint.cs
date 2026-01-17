
namespace Uracle.API.Endpoints.Contests
{
    public class GetAllContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/contests", async (ISender sender, HttpContext context) =>
            {
                var token = context.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetAllContestsQuery(
                    token
                ));
                if (result.IsFail)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                        detail: result.Message);                // Placeholder for creating a contest
                }
                // Placeholder for creating a contest
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}
