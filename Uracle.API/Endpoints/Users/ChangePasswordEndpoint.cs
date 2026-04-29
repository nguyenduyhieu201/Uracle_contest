
using System.Web.Http;
using static Uracle.API.Endpoints.Users.ForgetPasswordEndpoint;

namespace Uracle.API.Endpoints.Users
{
    public class ChangePasswordEndpoint : ICarterModule
    {

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/change-password", async ([FromBody] string oldPassword, [FromBody] string newPassword, [FromBody] string userId, ISender sender) =>
            {
                var command = new ChangePasswordCommand(oldPassword, newPassword, userId);
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
