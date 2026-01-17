namespace Uracle.API.Endpoints.Contests
{
    public class GetAllContestTeamsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/contests/{id}/teams", async (
                string id,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetContestTeamsQuery(id),
                    cancellationToken);
                if (!result.IsSuccess)
                {
                    // Tuỳ projet: có thể mapping error code cụ thể hơn
                    return Results.Problem(
                        detail: result.Message,
                        statusCode: StatusCodes.Status400BadRequest);
                }
                return Results.Ok(result.Value);
            })
            .RequireAuthorization()
            .WithName("GetContestTeams")
            .WithTags("Contests")
            .Produces<List<ContestTeamDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
