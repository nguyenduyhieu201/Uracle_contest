using Uracle.Domain.Models;

namespace Uracle.Application.Abstractions.Security
{
    public interface IJWTService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
    }
}
