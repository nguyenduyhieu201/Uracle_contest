
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs;
using static System.Net.WebRequestMethods;

namespace Uracle.API.Endpoints.Users
{
    public record UserLoginRequest(UserLoginDTO loginDto);
    public class LoginEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (UserLoginRequest request, ISender sender, HttpContext http) =>
            {
                var command = new UserLoginCommand(request.loginDto);
                var result = await sender.Send(command);
                if (result.IsFail)
                {
                    return Results.Json(
                            new { message = result.Message },
                            statusCode: StatusCodes.Status401Unauthorized
                        );
                }

                var accessCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,            // đặt true khi chạy HTTPS (production)
                    SameSite = SameSiteMode.Lax, // hoặc Strict nếu không cần gửi khi điều hướng cross-site
                    Path = "/",               // Access token dùng cho mọi API
                    Expires = DateTimeOffset.UtcNow.AddMinutes(5) // TTL ngắn
                };

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,               // production: true, current is false
                    SameSite = SameSiteMode.Strict, // chống CSRF tốt hơn cho refresh
                    Path = "/auth/refresh",      // chỉ gửi cookie này tới route refresh
                    Expires = DateTimeOffset.UtcNow.AddDays(10) // hoặc theo config
                };

                http.Response.Cookies.Append("AccessToken", result.Value.JwtToken, accessCookieOptions);
                http.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken, refreshCookieOptions);
                return Results.Ok(result);
            });
        }
    }
}
