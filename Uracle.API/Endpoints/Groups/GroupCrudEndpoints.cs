using Uracle.Application.Commands.GroupsCommand;
using Uracle.Application.Queries.GroupsQuery;

namespace Uracle.API.Endpoints.Groups
{
    public class GroupCrudEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/groups").RequireAuthorization();

            // GET /api/groups/:id
            group.MapGet("/{id}", async (string id, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetGroupByIdQuery(id, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            });

            // PUT /api/groups/:id
            group.MapPut("/{id}", async (string id, UpdateGroupRequest req, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new UpdateGroupCommand(id, token ?? "", req.Name, req.Description, req.IsPrivate), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(new { id = result.Value, message = "Group updated successfully" });
            });

            // DELETE /api/groups/:id
            group.MapDelete("/{id}", async (string id, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new DeleteGroupCommand(id, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.NoContent();
            });
        }

        public record UpdateGroupRequest(string? Name, string? Description, bool? IsPrivate);
    }
}
