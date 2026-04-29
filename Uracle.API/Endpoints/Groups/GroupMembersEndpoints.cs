using Uracle.Application.Commands.GroupsCommand;
using Uracle.Application.Queries.GroupsQuery;

namespace Uracle.API.Endpoints.Groups
{
    public class GroupMembersEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/groups").RequireAuthorization();

            // GET /api/groups/:id/members
            group.MapGet("/{id}/members", async (
                string id, ISender sender, HttpContext ctx, CancellationToken ct,
                int page = 1, int limit = 20) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetGroupMembersQuery(id, token ?? "", page, limit), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(new { members = result.Value, page, limit });
            });

            // POST /api/groups/:id/members
            group.MapPost("/{id}/members", async (
                string id, AddMemberRequest req, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new AddGroupMemberCommand(id, token ?? "", req.UserId, req.Role ?? "member"), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Created($"/api/groups/{id}/members", new { memberId = result.Value });
            });

            // DELETE /api/groups/:id/members/:userId
            group.MapDelete("/{id}/members/{userId}", async (
                string id, string userId, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new RemoveGroupMemberCommand(id, userId, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.NoContent();
            });

            // PUT /api/groups/:id/members/:userId/role
            group.MapPut("/{id}/members/{userId}/role", async (
                string id, string userId, UpdateRoleRequest req, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new UpdateMemberRoleCommand(id, userId, req.Role, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(new { message = "Member role updated successfully" });
            });

            // GET /api/groups/:id/role
            group.MapGet("/{id}/role", async (string id, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetUserRoleInGroupQuery(id, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(new { role = result.Value });
            });
        }

        public record AddMemberRequest(string UserId, string? Role);
        public record UpdateRoleRequest(string Role);
    }
}
