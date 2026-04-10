
using Uracle.Application.Commands.TeamsCommand;

namespace Uracle.API.Endpoints.Teams
{
    public class DeleteTeamMemberInContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // DELETE /api/teams/{teamId}/members/{userId}
            app.MapDelete("/api/teams/{teamId}/members/{userId}", async (
                string teamId,
                string userId,
                HttpContext context,
                ISender sender, 
                CancellationToken ct) =>
            {
                var token = context.Request.Cookies["AccessToken"];

                var command = new RemoveTeamMemberCommand(teamId, userId, token);
                var result = await sender.Send(command, ct);
                if (!result.IsSuccess)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                                       detail: result.Message);
                }
                return Results.NoContent();
            }).RequireAuthorization();

        }
    }
}
