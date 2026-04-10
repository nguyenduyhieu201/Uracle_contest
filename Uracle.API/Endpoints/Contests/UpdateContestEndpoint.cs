namespace Uracle.API.Endpoints.Contests
{
    public class UpdateContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/contests/{id}", async(string id, HttpContext context, ISender sender, ContestUpdateDto updateContestDto) =>
            {
                var token = context.Request.Cookies["AccessToken"];
                var command = new UpdateContestCommand(
                    id,
                    token,
                    updateContestDto
                );
                var result = await sender.Send(command);
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
