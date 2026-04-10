
namespace Uracle.API.Endpoints.Teams
{
    public class GetContestParticipantsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/contests/{id}/participants", async (
                string id,
                ISender sender,                      // nếu dùng MediatR
                CancellationToken cancellationToken) =>
            {
                var query = new GetContestParticipantsQuery(id);
                var result = await sender.Send(query, cancellationToken);
                if (result.IsFail)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                                       detail: result.Message);
                }
                return Results.Ok(result.Value);

            })
            .RequireAuthorization()
            .WithName("GetContestParticipants")
            .Produces<GetContestParticipantsResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
