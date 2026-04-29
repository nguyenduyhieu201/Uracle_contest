namespace Uracle.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<User?> GetUserByIdAsync(string Id, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id == Id);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            return user;
        }

        public async Task<User> GetByUserNameAsync(string username, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
            return user;
        }
        
        public async Task SetRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user != null)
            { 
                user.JwtRefreshToken = refreshToken;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Result<User?>> UpdateResetTokenAsync(string userId, string resetToken, DateTime expiresAt, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                return Result<User?>.Fail("User not found", ErrorCode.NotFound);
            }

            user.SetResetToken(resetToken, expiresAt);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<User?>.Success(user);
        }

        public async Task UpdateUserTokensAsync(string userId, StravaTokenResponse stravaResponse, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user != null)
            {
                user.AccessToken = stravaResponse.AccessToken;
                user.RefreshToken = stravaResponse.RefreshToken;
                user.ExpiresAt = stravaResponse.ExpiresAt;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> UpdatePasswordAsync(User user, string newPasswordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = newPasswordHash;
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<List<User>> SearchByNameAsync(string query, CancellationToken cancellationToken)
        {
            var lowerQuery = query.ToLower();
            return await _context.Users
                .Where(u => u.Username.ToLower().Contains(lowerQuery)
                         || (u.DisplayName != null && u.DisplayName.ToLower().Contains(lowerQuery))
                         || (u.Email != null && u.Email.ToLower().Contains(lowerQuery)))
                .Take(20)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateProfileAsync(string userId, string? displayName, string? email, string? bio, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null) return false;
            if (displayName != null) user.DisplayName = displayName;
            if (email != null) user.Email = email;
            if (bio != null) user.Bio = bio;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<WorkoutActivity>> GetWorkoutActivitiesByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.WorkoutActivities
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.StartDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetByStravaIdAsync(long stravaId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.StravaId == stravaId, cancellationToken);
        }

        public async Task ClearStravaTokensAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null) return;

            user.AccessToken = null;
            user.RefreshToken = null;
            user.ExpiresAt = null;
            user.StravaId = null;
            user.StravaProfile = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
