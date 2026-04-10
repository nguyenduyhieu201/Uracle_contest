using Uracle.Application.DTOs.StravasDto;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user, CancellationToken cancellationToken);
        public Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
        public Task<User> GetByUserNameAsync(string username, CancellationToken cancellationToken);
        public Task SetRefreshTokenAsync(string UserId, string refreshToken, CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(string Id, CancellationToken cancellationToken);
        Task UpdateUserTokensAsync(string Id, StravaTokenResponse stravaResponse, CancellationToken cancelToken);
        Task<Result<User?>> UpdateResetTokenAsync(string userId, string resetToken, DateTime expiresAt, CancellationToken cancellationToken);
    }
}
