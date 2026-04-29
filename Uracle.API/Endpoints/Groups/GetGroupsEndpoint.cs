
using Uracle.Application.DTOs.GroupsDto;
using Uracle.Application.Queries.GroupsQuery;

namespace Uracle.API.Endpoints.Groups
{
    public class GetGroupsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/groups").RequireAuthorization();

            // GET /api/groups - Get All Groups
            group.MapGet("/", async (ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetAllGroupsQuery(token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            })
            .WithName("GetAllGroups")
            .Produces<GroupsListResponse>(200)
            .Produces(401)
            .Produces(500);
        }
    }
}
