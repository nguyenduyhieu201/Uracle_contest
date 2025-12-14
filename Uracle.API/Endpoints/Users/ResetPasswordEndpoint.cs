
namespace Uracle.API.Endpoints.Users
{
    public class ResetPasswordEndpoint : ICarterModule
    {
        public record ForgotPasswordCommand(string Email);
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/forgot-password", async (ForgotPasswordCommand request, ISender sender) =>
            {
                var command = new ForgotPasswordCommand(request.Email);
                var result = await sender.Send(command);
                //if (result.IsFail) return Results.BadRequest(result.Message);
                return Results.Ok(result);
            });
        }
    }
}
