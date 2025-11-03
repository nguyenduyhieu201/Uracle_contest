
namespace Uracle.API.Endpoints.Users
{
    public class LogoutEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/logout", async (HttpContext http, ISender sender) =>
            {
                try
                {
                    // Xoá cookie AccessToken và RefreshToken
                    if (http.Request.Cookies.ContainsKey("AccessToken"))
                    {
                        http.Response.Cookies.Delete("AccessToken");
                    }
                    if (http.Request.Cookies.ContainsKey("RefreshToken"))
                    {
                        http.Response.Cookies.Delete("RefreshToken");
                    }
                    return Results.Ok(new { Message = "Logged out successfully." });
                }
                catch (Exception ex)
                {
                    return Results.Problem("An error occurred during logout: " + ex.Message);
                }
            });
        }
    }
}
