using SharedKernel.Domains;

namespace Uracle.API.Endpoints.Contests
{
    public class AddParticipantToContestEndpoint : ICarterModule
    {
        public record AddParticipantRequestDto(string ParticipantId);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/contests/{contestId}/participants", async (
                string contestId,
                AddParticipantRequestDto request,
                HttpContext context,
                ISender sender,                 // hoặc ICommandBus
                CancellationToken cancellationToken) =>
            {
                var token = context.Request.Cookies["AccessToken"];

                var command = new AddParticipantToContestCommand(
                    ContestId: contestId,
                    ParticipantId: request.ParticipantId);
                var result = await sender.Send(command, cancellationToken);
                if (result.IsFail)
                {
                    return Results.Problem(
                        detail: result.Message,
                        statusCode: (int)result.ErrorCode);
                }
                return Results.Ok(result.Value);
            })
                .RequireAuthorization();

        }
    }
}
