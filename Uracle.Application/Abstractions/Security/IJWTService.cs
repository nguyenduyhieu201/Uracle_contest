
namespace Uracle.Application.Abstractions.Security
{
    public interface IJwtService
    {
        Task<Result<string>> ValidateUserAsync(string jwtToken, CancellationToken cancellation = default); 
        string GenerateToken(User user);
        string GenerateRefreshToken(User user);
        string GeneratePasswordResetToken();

    }
}
