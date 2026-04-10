



using Uracle.Application.DTOs.UsersDto;

namespace Uracle.API.Endpoints.Users
{
    public class RegisterEndpoint : ICarterModule
    {
        public record RegisterUserRequest(UserRegisterDTO registerDto);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/register", async (RegisterUserRequest request, ISender sender) =>
            {
                var command = new UserRegisterCommand(request.registerDto);
                var result = await sender.Send(command);
                if (result.IsFail)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                                 detail: result.Message);
                }
                return Results.Ok(result.Value);
            });
        }
    }
}
