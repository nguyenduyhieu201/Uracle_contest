using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Uracle.API.Endpoints.Contests
{
    public class CreateContestEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("api/contest/create", async (ContestCreateRequest contestCreateDto, ISender sender, HttpContext context) =>
            {
                var token = context.Request.Cookies["AccessToken"];
                var command = new ContestCreateCommand(token,
                                contestCreateDto
                            );
                var result = await sender.Send(command);
                if (result.IsFail)
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                            detail: result.Message);                // Placeholder for creating a contest
                return Results.Ok(result.Value);
            }).RequireAuthorization();  
        }
    }
}
