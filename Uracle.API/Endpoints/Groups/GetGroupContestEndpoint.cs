
namespace Uracle.API.Endpoints.Groups
{
    public class GetGroupContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/groups")
                           .RequireAuthorization();
            // GET /api/groups/{id}/contests
            group.MapGet("/{id}/contests", async (
                string id,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GetGroupContestsQuery(id),
                    ct);
                if (!result.IsSuccess)
                {
                    return Results.Problem(
                        detail: result.Message ?? "Internal server error while fetching contests for group",
                        statusCode: (int)result.ErrorCode);
                }
                return Results.Ok(result.Value);
            });
        }
    }
}
