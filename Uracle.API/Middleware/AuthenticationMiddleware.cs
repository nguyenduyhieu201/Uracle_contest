
namespace Uracle.API.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _jwtSecret;
        
        public AuthenticationMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _jwtSecret = config["Jwt:Key"];
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string token = null;
            // 1 Bỏ qua middleware nếu route nằm trong danh sách ngoại lệ
            var path = context.Request.Path.Value?.ToLower();
            if (_excludedPaths.Any(p => path != null && path.StartsWith(p)))
            {
                await _next(context);
                return;
            }
            // 1️⃣ Lấy token từ cookie
            if (context.Request.Cookies.TryGetValue("AccessToken", out var cookieToken))
            {
                token = cookieToken;
            }

            // 2️⃣ Nếu chưa có thì lấy từ header Authorization: Bearer <token>
            if (string.IsNullOrEmpty(token))
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            // Nếu không thấy token thì trả về lỗi 401 Unauthorized
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "No token provided" });
                return;
            }

            // 3️⃣ Xác thực token
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                context.User = principal;
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid token" });
                return;
            }

            await _next(context);
        }
        private readonly string[] _excludedPaths = new[]
        {
            "/",
            "/login",
            "/register",
            "/health"
        };
    }
}
