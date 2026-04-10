using Uracle.Application.Commands.TeamsCommand;

namespace Uracle.API.Endpoints.Contests
{
    public class BulkAddMemberToTeamEndpoint : ICarterModule
    {
        public record AddTeamMembersBulkRequest(List<string> UserIds);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // POST /api/contests/teams/{teamId}/members/bulk
            app.MapPost("/api/teams/{teamId}/members/bulk", async (
                string teamId,
                AddTeamMembersBulkRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new AddTeamMembersBulkCommand(teamId, request.UserIds);
                var result = await sender.Send(command, ct);
                if (!result.IsSuccess)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                                       detail: result.Message);
                }
                
                return Results.Ok(result.Value);
            })
            .RequireAuthorization();
        }
    }    
}

