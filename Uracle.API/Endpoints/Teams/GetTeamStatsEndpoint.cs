using Uracle.Application.Queries.TeamsQuery;

namespace Uracle.API.Endpoints.Teams
{
    public class GetTeamStatsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // GET /api/teams/:teamId/stats
            app.MapGet("api/teams/{teamId}/stats", async (
                string teamId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetTeamStatsQuery { TeamId = teamId }, ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();

            // GET /api/contests/:contestId/teams/:teamId/stats
            app.MapGet("api/contests/{contestId}/teams/{teamId}/stats", async (
                string contestId, string teamId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetTeamStatsQuery { TeamId = teamId }, ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}
