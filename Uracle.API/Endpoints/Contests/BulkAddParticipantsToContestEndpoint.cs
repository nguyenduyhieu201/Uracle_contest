using Uracle.Application.DTOs.ContestsDto;

namespace Uracle.API.Endpoints.Contests
{

    public sealed class AddMultipleParticipantsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/contests/{contestId}/participants/bulk",
                async (string contestId,
                        AddMultipleParticipantsRequest body,
                        HttpContext httpContext,
                        ISender sender,
                        CancellationToken cancellationToken) =>
                {
                    var token = httpContext.Request.Cookies["AccessToken"];

                    var command = new AddMultipleParticipantsCommand(
                                                contestId,
                                                token,
                                                body.ParticipantIds);
                    var result = await sender.Send(command, cancellationToken);
                    if (!result.IsSuccess)
                    {
                        var error = result.Message ?? "Failed to add participants to contest";
                        // Map một số case giống Node: thường trả 400 cho lỗi business

                        return Results.Problem(statusCode: (int)result.ErrorCode,
                                                detail: error);
                    }
                    return Results.Ok(result.Value);
                })
            .RequireAuthorization()
            .WithName("AddMultipleContestParticipants")
            .WithTags("Contests")
            .Produces<AddMultipleParticipantsResultDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}

