namespace Uracle.Application.Abstractions.Services
{
    public interface IStravaService
    {
        public Task<Result<string>> HandleAuthorizeUrl(string token);
        public Task<Result<User?>> HandleStravaCallback(string code, string? error, string userId, CancellationToken cancellationToken);
    }
}
