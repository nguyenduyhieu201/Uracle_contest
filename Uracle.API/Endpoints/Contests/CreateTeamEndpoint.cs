namespace Uracle.API.Endpoints.Contests
{
    public record CreateTeamRequest(
        string Name,
        string? Description
    );
    public class CreateTeamEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/contests/{contestId}/teams", async (ISender sender, CreateTeamRequest request, string contestId, CancellationToken cancellationToken) =>
            {
                var command = new CreateTeamCommand(contestId, request.Name, request.Description);
                var result = await sender.Send(command, cancellationToken);
                // Placeholder for creating a team within a contest
                if (result.IsFail)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                            detail: result.Message);                // Placeholder for creating a contest
                }
                return Results.Ok();
            }).RequireAuthorization();
        }
    }
}
