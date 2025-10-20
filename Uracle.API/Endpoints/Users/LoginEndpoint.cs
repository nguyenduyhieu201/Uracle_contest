
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs;

namespace Uracle.API.Endpoints.Users
{
    public record UserLoginRequest(UserLoginDTO loginDto);
    public class LoginEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (UserLoginRequest request, ISender sender) =>
            {
                var command = new UserLoginCommand(request.loginDto);
                var result = await sender.Send(command);
                return Results.Ok(result);
            });
        }
    }
}
