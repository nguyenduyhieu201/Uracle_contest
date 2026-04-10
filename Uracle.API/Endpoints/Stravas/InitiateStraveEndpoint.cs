namespace Uracle.API.Endpoints.Users
{
    public class InitiateStraveEndpoint : ICarterModule
    {
         
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/strava/connect", async (ISender sender, HttpContext httpContext) => 
            {
                var token = httpContext.Request.Cookies["AccessToken"];
                var result = await sender.Send(new InitiateStravaQuery(token ?? string.Empty));
                if (result.IsFail)
                    return Results.Problem(statusCode: (int)result.ErrorCode,
                                                       detail: result.Message);
                var url = result.Value.AuthorizationUrl;
                return Results.Redirect(url);
            }).RequireAuthorization();

        }
    }
}
