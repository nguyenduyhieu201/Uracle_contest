using Uracle.Application.Commands.GroupsCommand;
using Uracle.Application.Queries.GroupsQuery;

namespace Uracle.API.Endpoints.Groups
{
    public class GroupJoinRequestsEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/groups").RequireAuthorization();

            // POST /api/groups/:id/join
            group.MapPost("/{id}/join", async (string id, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new RequestToJoinGroupCommand(id, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Created($"/api/groups/{id}/join", new { message = "Join request sent successfully" });
            });

            // GET /api/groups/:id/requests
            group.MapGet("/{id}/requests", async (string id, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetPendingJoinRequestsQuery(id, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            });

            // POST /api/groups/:id/requests/:userId/approve
            group.MapPost("/{id}/requests/{userId}/approve", async (
                string id, string userId, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new ApproveJoinRequestCommand(id, userId, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(new { message = "Join request approved successfully" });
            });

            // POST /api/groups/:id/requests/:userId/reject
            group.MapPost("/{id}/requests/{userId}/reject", async (
                string id, string userId, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new RejectJoinRequestCommand(id, userId, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(new { message = "Join request rejected successfully" });
            });

            // GET /api/groups/:id/join-status
            group.MapGet("/{id}/join-status", async (string id, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetJoinStatusQuery(id, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(new { hasJoinRequest = result.Value });
            });

            // DELETE /api/groups/:id/join
            group.MapDelete("/{id}/join", async (string id, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new RevokeJoinRequestCommand(id, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(new { message = "Join request revoked successfully" });
            });
        }
    }
}
