
using Uracle.Application.Commands.ContestsCommand;
using Uracle.Application.Queries.ContestsQuery;

namespace Uracle.API.Endpoints.Contests
{
    public class GetDetailContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/contest/{id}", async (string contestId, ISender sender, HttpContext context) =>
            {
                var token = context.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetContestByIdQuery(
                    contestId,
                    token
                ));
                if (result.IsFail)
                {
                    Results.BadRequest(result.Message);
                }

                // Placeholder for creating a contest
                return Results.Ok(result);
            }).RequireAuthorization();
        }
    }
}
