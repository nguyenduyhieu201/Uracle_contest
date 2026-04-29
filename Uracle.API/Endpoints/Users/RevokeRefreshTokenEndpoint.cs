
using Uracle.Application.Commands.UsersCommand;
using Uracle.Infrastructure.Options;

namespace Uracle.API.Endpoints.Users
{
    public class RefreshTokenEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/refresh-token", async (ISender sender, HttpContext http, IOptions<AuthCookieOptions> cfg, CancellationToken ct) =>
            {
                var refreshToken = http.Request.Cookies["RefreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Results.Json(new { message = "Refresh token is required" }, statusCode: StatusCodes.Status400BadRequest);

                var result = await sender.Send(new RefreshTokenCommand(refreshToken), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);

                var accessOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = cfg.Value.Secure,
                    SameSite = cfg.Value.SameSiteAccess == "Strict" ? SameSiteMode.Strict : SameSiteMode.Lax,
                    Path = cfg.Value.AccessToken.Path,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(cfg.Value.AccessToken.ExpiresInHours)
                };
                var refreshOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = cfg.Value.Secure,
                    SameSite = cfg.Value.SameSiteRefresh == "Strict" ? SameSiteMode.Strict : SameSiteMode.Lax,
                    Path = cfg.Value.RefreshToken.Path,
                    Expires = DateTimeOffset.UtcNow.AddDays(cfg.Value.RefreshToken.ExpiresInDays)
                };

                http.Response.Cookies.Append("AccessToken", result.Value!.AccessToken, accessOptions);
                http.Response.Cookies.Append("RefreshToken", result.Value.NewRefreshToken, refreshOptions);
                return Results.Ok(new { message = "Tokens refreshed successfully" });
            });
        }
    }
}
