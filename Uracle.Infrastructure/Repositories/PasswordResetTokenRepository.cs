

namespace Uracle.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public PasswordResetTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task RevokeValidTokensForUserAsync(string userId, CancellationToken cancellationToken)
        {
            var validTokens = await GetValidTokensByUserIdAsync(userId, cancellationToken);
            validTokens.ResetToken = null;
            validTokens.ResetTokenExpiry = null;
            await _context.SaveChangesAsync(cancellationToken);
        }
        private async Task<User?> GetValidTokensByUserIdAsync(
                                    string userId,
                                    CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == userId &&
                    u.ResetToken != null &&
                    u.ResetTokenExpiry != null &&
                    u.ResetTokenExpiry > DateTime.UtcNow,
                    cancellationToken);
        }
    }
}
