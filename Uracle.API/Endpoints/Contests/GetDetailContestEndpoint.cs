

namespace Uracle.API.Endpoints.Contests
{
    public class GetDetailContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/contests/{contestId}", async (string contestId, ISender sender, HttpContext context) =>
            {
                var result = await sender.Send(new GetContestByIdQuery(
                    contestId
                ));
                if (result.IsFail)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                                        detail: result.Message);                

                }
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}
