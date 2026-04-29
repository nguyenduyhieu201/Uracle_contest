namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        public Task<bool> RevokeValidTokensForUserAsync(string userId, CancellationToken cancellationToken);
    }
}
