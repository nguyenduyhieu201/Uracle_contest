using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.DTOs;

namespace Uracle.Infrastructure.Repositories
{
    public class StravaRepository : IStravaRepository
    {
        private readonly ApplicationDbContext _context;
        public StravaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SetStravaProfile(string userId, StravaAthleteDTO athlete, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            var strava = await _context.StravaProfiles.FirstOrDefaultAsync(strava => strava.Id == athlete.Id, cancellationToken);
            if (user == null)
            {
                return;
            }
            if (strava == null)
            {
                strava = new StravaProfile
                {
                    Username = athlete.Username,
                    Firstname = athlete.Firstname ?? string.Empty,
                    Lastname = athlete.Lastname ?? string.Empty
                };

                await _context.StravaProfiles.AddAsync(strava);
            }
            else
            {
                strava.Username = athlete.Username;
                strava.Firstname = athlete.Firstname ?? string.Empty;
                strava.Lastname = athlete.Lastname ?? string.Empty;
            }

            user.StravaId = athlete.Id;
            user.StravaProfile = strava;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
