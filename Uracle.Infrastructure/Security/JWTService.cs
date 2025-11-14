using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Uracle.Infrastructure.Security
{

    public class JWTService : IJWTService
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

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
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
                return Result<string>.Fail("Token cannot be empty");
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(jwtToken, _validationParams, out _);
            string userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString();

            if (string.IsNullOrEmpty(userId))
                return Result<string>.Fail("cannot parse userId");

            var user =  await _users.FindByIdAsync(userId, cancellation);
            if (user is null) return Result<string>.Fail("cannot find current user");
            return Result<string>.Success(userId);
        }

  
    }
}
