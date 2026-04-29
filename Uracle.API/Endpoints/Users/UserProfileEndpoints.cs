using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs.UsersDto;
using Uracle.Application.Queries.UsersQuery;

namespace Uracle.API.Endpoints.Users
{
    public class UserProfileEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // GET /api/user/profile
            app.MapGet("api/user/profile", async (ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetUserProfileQuery(token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();

            // PUT /api/user/profile
            app.MapPut("api/user/profile", async (UpdateUserProfileRequest req, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new UpdateUserProfileCommand(token ?? "", req.DisplayName, req.Email, req.Bio), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();

            // GET /api/users/search?query=...
            app.MapGet("api/users/search", async (string query, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new SearchUsersQuery(query), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();

            // GET /api/users/:id
            app.MapGet("api/users/{id}", async (string id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetUserByIdQuery(id), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();

            // GET /api/user/activities
            app.MapGet("api/user/activities", async (ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetUserActivitiesQuery(token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();

            // GET /api/user/teams
            app.MapGet("api/user/teams", async (ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetUserTeamsQuery(null, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();

            // GET /api/users/:userId/teams
            app.MapGet("api/users/{userId}/teams", async (string userId, ISender sender, HttpContext ctx, CancellationToken ct) =>
            {
                var token = ctx.Request.Cookies["AccessToken"];
                var result = await sender.Send(new GetUserTeamsQuery(userId, token ?? ""), ct);
                if (result.IsFail)
                    return Results.Problem(detail: result.Message, statusCode: (int)result.ErrorCode);
                return Results.Ok(result.Value);
            }).RequireAuthorization();
        }
    }
}
