
using Microsoft.AspNetCore.Http;
using Uracle.Application.Commands.ContestsCommand;
using Uracle.Application.DTOs.ContestDto;
using Uracle.Application.Queries.UsersQuery;

namespace Uracle.API.Endpoints.Contests
{
    public class CreateContestEndpoint : ICarterModule
    {
        public record ContestCreateRequest (ContestCreateRequestDto contestCreateDto);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/contest/create", async (ContestCreateRequest contestRequest, ISender sender, HttpContext context) =>
            {
                var token = context.Request.Cookies["AccessToken"];
                var command = new ContestCreateCommand(token,
                                contestRequest.contestCreateDto
                            );
                var result = await sender.Send(command);
                // Placeholder for creating a contest
                return Results.Ok(result);
            }).RequireAuthorization();  
        }
    }
}
