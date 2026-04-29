
namespace Uracle.API.Endpoints.Users
{
    public class ForgetPasswordEndpoint : ICarterModule
    {
        public record ForgotPasswordCommand(string Email);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/forgot-password", async (ForgotPasswordCommand request, ISender sender) =>
            {
                var command = new UserForgetPasswordCommand(request.Email);
                var result = await sender.Send(command);
                if (result.IsFail)
                {
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                                       detail: result.Message);
                }
                return Results.NoContent();
            });
        }
    }
}
