using Uracle.Application.DTOs.StravasDto;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user, CancellationToken cancellationToken);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User> GetByUserNameAsync(string username, CancellationToken cancellationToken);
        Task SetRefreshTokenAsync(string UserId, string refreshToken, CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(string Id, CancellationToken cancellationToken);
        Task UpdateUserTokensAsync(string Id, StravaTokenResponse stravaResponse, CancellationToken cancelToken);
        Task<Result<User?>> UpdateResetTokenAsync(string userId, string resetToken, DateTime expiresAt, CancellationToken cancellationToken);
        Task<User> UpdatePasswordAsync(User user, string newPasswordHash, CancellationToken cancellationToken);

        // New methods
        Task<List<User>> SearchByNameAsync(string query, CancellationToken cancellationToken);
        Task<bool> UpdateProfileAsync(string userId, string? displayName, string? email, string? bio, CancellationToken cancellationToken);
        Task<List<WorkoutActivity>> GetWorkoutActivitiesByUserIdAsync(string userId, CancellationToken cancellationToken);

        // Thêm vào Uracle.Application/Abstractions/Interfaces/IUserRepository.cs:
        Task<User?> GetByStravaIdAsync(long stravaId, CancellationToken cancellationToken = default);
        Task ClearStravaTokensAsync(string userId, CancellationToken cancellationToken = default);
    }
}
