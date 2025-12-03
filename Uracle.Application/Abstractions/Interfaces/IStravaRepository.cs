namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IStravaRepository
    {
        public Task SetStravaProfile(string userId, StravaAthleteDTO athlete, CancellationToken cancellationToken);
    }
}
