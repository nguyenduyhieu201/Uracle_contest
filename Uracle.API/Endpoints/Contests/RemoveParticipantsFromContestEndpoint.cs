
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Uracle.API.Endpoints.Contests
{
    public class RemoveParticipantsFromContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/contests/{contestId}/participants/{participantId}",
                async (string contestId,
                       string participantId,
                       HttpContext httpContext,
                       ISender sender,
                       CancellationToken cancellationToken) =>
                {
                    var token = httpContext.Request.Cookies["AccessToken"];
                    
                    var command = new RemoveContestParticipantCommand(
                                                contestId,
                                                participantId,
                                                token);
                    var result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        return Results.Problem(statusCode: (int)result.ErrorCode,
                                                 detail: result.Message);
                    }
                    return Results.NoContent();
                })
            .RequireAuthorization()
            .WithName("RemoveContestParticipant")
            .WithTags("Contests")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
        }
    }
}
