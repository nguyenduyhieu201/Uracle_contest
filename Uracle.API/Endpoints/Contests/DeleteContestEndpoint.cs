namespace Uracle.API.Endpoints.Contests
{
    public class DeleteContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/contests/{contestId}", async (string contestId, ISender sender, HttpContext context) =>
            {
                var token = context.Request.Cookies["AccessToken"];
                var command = new DeleteContestCommand(
                    contestId,
                    token
                );
                var result = await sender.Send(command);
                if (result.IsFail)
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                            detail: result.Message);                
                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
