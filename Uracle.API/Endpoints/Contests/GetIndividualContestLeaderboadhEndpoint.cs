namespace Uracle.API.Endpoints.Contests
{
    public class GetIndividualContestLeaderboadhEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/contests/{contestId}/leaderboard",
                async (string contestId,
                       ISender sender,
                       CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                    new GetIndividualContestLeaderboardQuery(contestId),
                    cancellationToken);
                    if (!result.IsSuccess)
                    {
                        var error = result.Message ?? "Failed to get contest leaderboard";
                        return Results.Problem(statusCode: (int)result.ErrorCode,
                        detail: error);
                    }
                    return Results.Ok(result.Value);
                })
            .RequireAuthorization()
            .WithName("GetIndividualContestLeaderboard")
            .WithTags("Contests")
            .Produces<IReadOnlyList<IndividualContestLeaderboardItemDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
