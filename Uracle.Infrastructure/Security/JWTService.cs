

namespace Uracle.Infrastructure.Security
{
    public class JWTService : IJwtService
    {
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IUserRepository _users;

        private readonly TokenValidationParameters _validationParams;

        public JWTService(IUserRepository users, IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions;
            _users = users;
            var opts = jwtOptions.Value;
            _validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(opts.Key)),
                ValidateIssuer = !string.IsNullOrEmpty(opts.Issuer),
                ValidIssuer = opts.Issuer,
                ValidateAudience = !string.IsNullOrEmpty(opts.Audience),
                ValidAudience = opts.Audience,
            };
        }


        public string GenerateRefreshToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim("type", "refresh")
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenExpiryInDays), // hết hạn sau 7 ngày
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Value.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Value.Issuer,
                audience: _jwtOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtOptions.Value.ExpiryInHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Result<string>> ValidateUserAsync(string jwtToken, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
                return Result<string>.Fail("Token cannot be empty", ErrorCode.BadRequest);
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(jwtToken, _validationParams, out _);
            string userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            if (string.IsNullOrEmpty(userId))
                return Result<string>.Fail("cannot parse userId", ErrorCode.BadRequest);

            var user =  await _users.GetUserByIdAsync(userId, cancellation);
            if (user is null) return Result<string>.Fail("cannot find current user", ErrorCode.NotFound);
            return Result<string>.Success(userId);
        }

        public string GeneratePasswordResetToken()
        {
            return GenerateSecureToken(32);
        }

        private static string GenerateSecureToken(int length)
        {
            var randomBytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "")
                .Substring(0, Math.Min(length, 32));
        }
    }
}
