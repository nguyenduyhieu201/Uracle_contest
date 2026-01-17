using Uracle.Application.DTOs.ContestsDto;

namespace Uracle.API.Endpoints.Contests
{
    public class GetContestTeamLeaderboardEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/contests/{id}/team-leaderboard",
                async (string id,
                       HttpRequest request,
                       ISender mediator,
                       CancellationToken cancellationToken) =>
                {
                    var metric = request.Query["metric"].ToString();
                    if (string.IsNullOrWhiteSpace(metric))
                        metric = "totalDistance";
                    var limitStr = request.Query["limit"].ToString();
                    var limit = 50;
                    if (!string.IsNullOrWhiteSpace(limitStr) &&
                    int.TryParse(limitStr, out var parsed))
                    {
                        limit = parsed;
                    }
                    var query = new GetContestTeamLeaderboardQuery(id, metric, limit);
                    var result = await mediator.Send(query, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        return Results.Problem(statusCode: (int)result.ErrorCode,
                                                detail: result.Message);                // Placeholder for creating a contest
                    }
                    return Results.Ok(result.Value);
                })
            .RequireAuthorization()
            .WithName("GetContestTeamLeaderboard")
            .WithTags("Contests")
            .Produces<TeamLeaderboardResponseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
