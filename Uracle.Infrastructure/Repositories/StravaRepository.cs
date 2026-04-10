
namespace Uracle.Infrastructure.Repositories
{
    public class StravaRepository : IStravaRepository
    {
        private readonly ApplicationDbContext _context;
        public StravaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SetStravaProfile(
            string userId,
            StravaAthleteDTO athlete,
            CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                return;
            }

            // Init nếu chưa có StravaProfile
            if (user.StravaProfile == null)
            {
                user.StravaProfile = new StravaProfile();
            }

            // Update dữ liệu profile
            user.StravaProfile.Username = athlete.Username;
            user.StravaProfile.Firstname = athlete.Firstname ?? string.Empty;
            user.StravaProfile.Lastname = athlete.Lastname ?? string.Empty;

            // Update StravaId trên User (Aggregate Root)
            user.StravaId = athlete.Id;

            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
