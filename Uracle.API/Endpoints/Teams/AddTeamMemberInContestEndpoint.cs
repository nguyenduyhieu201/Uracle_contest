
using Azure.Core;
using Newtonsoft.Json.Linq;
using Uracle.Application.Commands.TeamsCommand;
using Uracle.Domain.Models;

namespace Uracle.API.Endpoints.Teams
{
    public class AddTeamMemberInContestEndpoint : ICarterModule
    {
        public record AddTeamMemberRequest(string participantId);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/teams/{teamId}/members", async (
                HttpContext context,
                AddTeamMemberRequest addTeamMemberRequest,
                string teamId,
                ISender sender,
                CancellationToken ct) =>
            {
                var token = context.Request.Cookies["AccessToken"];

                var command = new AddTeamMemberCommand(token, addTeamMemberRequest.participantId, teamId);
                var result = await sender.Send(command, ct);
                if (result.IsFail)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                           detail: result.Message);
                }
                return Results.Ok(new { message = "Member added successfully" });  
            })
            .RequireAuthorization();
        }
    }
}
