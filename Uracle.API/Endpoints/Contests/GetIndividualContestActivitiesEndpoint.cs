using Uracle.Application.DTOs.ContestsDto;
using Uracle.Domain.Models.Contests;

namespace Uracle.API.Endpoints.Contests
{
    public class GetIndividualContestActivitiesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/contests/{contestId}/users/{userId}/activities",
                async (string contestId,
                       string userId,
                       ISender sender,
                       CancellationToken cancellationToken) =>
                {
                    var result = await sender.Send(
                                            new GetIndividualContestActivitiesQuery(contestId, userId),
                                            cancellationToken);
                    if (!result.IsSuccess)
                    {
                        return Results.Problem(statusCode: (int)result.ErrorCode,
                                                detail: result.Message);                
                    }
                    return Results.Ok(result.Value);
                })
            .RequireAuthorization()
            .WithName("GetIndividualContestActivities")
            .WithTags("Contests")
            .Produces<List<IndividualContestActivityDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
