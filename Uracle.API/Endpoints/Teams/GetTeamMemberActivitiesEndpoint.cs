
using Uracle.Application.Commands.TeamsCommand;
using Uracle.Application.Queries.TeamsQuery;

namespace Uracle.API.Endpoints.Teams
{
    public class GetTeamMemberActivitiesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            
            app.MapGet("/api/teams/{teamId}/members/{userId}/activities",
                        async (string teamId, string userId, ISender sender, HttpContext context, CancellationToken cancellationToken) =>
            {

                var command = new GetTeamMemberActivitiesQuery(teamId, userId);
                var result = await sender.Send(command, cancellationToken);
                if (!result.IsSuccess)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                                       detail: result.Message);
                }
                return Results.Ok(result.Value);
            }).RequireAuthorization();

        }
    }
}
