

namespace Uracle.Infrastructure.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public PasswordResetTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> RevokeValidTokensForUserAsync(string userId, CancellationToken cancellationToken)
        {
            var validTokens = await GetValidTokensByUserIdAsync(userId, cancellationToken);
            if (validTokens == null)
            {
                return false;
            }
            validTokens.ResetToken = null;
            validTokens.ResetTokenExpiry = null;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        private async Task<User?> GetValidTokensByUserIdAsync(
                                    string userId,
                                    CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == userId, 
                    cancellationToken);
        }
    }
}
