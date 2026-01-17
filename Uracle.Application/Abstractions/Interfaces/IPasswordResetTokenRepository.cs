namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        public Task RevokeValidTokensForUserAsync(string userId, CancellationToken cancellationToken);
    }
}
