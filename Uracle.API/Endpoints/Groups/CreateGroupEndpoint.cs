

using System.Security.Claims;

namespace Uracle.API.Endpoints.Groups
{
    public class CreateGroupEndpoint : ICarterModule
    {
        
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/groups")
                       .RequireAuthorization();
            group.MapPost("/", async (
                [FromBody] CreateGroupRequest request,
                ISender sender,
                HttpContext context, 
                CancellationToken ct) =>
            {
                string? userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await sender.Send(
                    new CreateGroupCommand(request.Name, request.Description, request.IsPrivacy, userId),
                    ct);
                if (!result.IsSuccess)
                {
                    return Results.Problem(
                        detail: result.Message ?? "Internal server error while fetching contests for group",
                        statusCode: (int)result.ErrorCode);
                }
                return Results.Ok(result);
            });
        }
    }
}
