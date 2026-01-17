
using Uracle.Application.Queries.TeamsQuery;

namespace Uracle.API.Endpoints.Teams
{
    public class GetTeamByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/teams/{id}", async (
                string id,
                ISender sender,                      // nếu dùng MediatR
                CancellationToken cancellationToken) =>
            {
                var query = new GetTeamByIdQuery(id);
                var result = await sender.Send(query, cancellationToken);
                if (result.IsFail)
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
