using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.ValueObjects;

namespace Uracle.Infrastructure.Data.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<User> Users => new List<User>
        {
            new User
            {
                Id = UserId.Of(Guid.NewGuid()),
                Username = "admin",
                Email = "admin@example.com",
                DisplayName = "Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
            },
            new User
            {
                Id = UserId.Of(Guid.NewGuid()),
                Username = "user",
                Email = "user@example.com",
                DisplayName = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123")
            }
        };
        public static IEnumerable<StravaProfile> StravaProfiles => new List<StravaProfile>
        {
            new StravaProfile { Id = 1001, Username = "strava_admin", Firstname = "System", Lastname = "Admin" },
            new StravaProfile { Id = 1002, Username = "strava_user",  Firstname = "Default", Lastname = "User" }
        };
    }
}
