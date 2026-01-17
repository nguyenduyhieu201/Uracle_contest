using Uracle.Application.DTOs.ContestsDto;

namespace Uracle.API.Endpoints.Contests
{
    public class GetAvaibleParticipantsContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/contests/{contestId}/available-participants",
                async (string contestId,
                       ISender mediator,
                       CancellationToken cancellationToken) =>
                {

                    var result = await mediator.Send(
                                    new GetAvailableContestParticipantsQuery(contestId),
                                    cancellationToken);
                    if (result.IsFail){
                        return Results.Problem(
                            statusCode: (int) result.ErrorCode,
                            detail: result.Message);
                    }
                    return Results.Ok(result.Value);
                })
            .RequireAuthorization()
            .WithName("GetAvailableContestParticipants")
            .WithTags("Contests")
            .Produces<IReadOnlyList<AvailableContestParticipantDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
