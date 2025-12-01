
using Microsoft.Extensions.Options;
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs;
using Uracle.Infrastructure.Options;
using static System.Net.WebRequestMethods;

namespace Uracle.API.Endpoints.Users
{
    public record UserLoginRequest(UserLoginDTO loginDto);
    public class LoginEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (UserLoginRequest request, ISender sender, HttpContext http, IOptions<AuthCookieOptions> cfg) =>
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
                    Secure = cfg.Value.Secure,
                    SameSite = cfg.Value.SameSiteAccess == "Strict"
                                 ? SameSiteMode.Strict
                                 : SameSiteMode.Lax,
                    Path = cfg.Value.AccessToken.Path,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(cfg.Value.AccessToken.ExpiresInHours)
                };

                var refreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = cfg.Value.Secure,
                    SameSite = cfg.Value.SameSiteRefresh == "Strict"
                        ? SameSiteMode.Strict
                        : SameSiteMode.Lax,
                    Path = cfg.Value.RefreshToken.Path,
                    Expires = DateTimeOffset.UtcNow.AddDays(cfg.Value.RefreshToken.ExpiresInDays)
                };

                http.Response.Cookies.Append("AccessToken", result.Value.JwtToken, accessCookieOptions);
                http.Response.Cookies.Append("RefreshToken", result.Value.RefreshToken, refreshCookieOptions);
                return Results.Ok(result);
            });
        }
    }
}
