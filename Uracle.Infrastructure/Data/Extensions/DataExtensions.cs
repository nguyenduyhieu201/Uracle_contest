using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Infrastructure.Data.Extensions
{
    
    public static class DataExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.MigrateAsync().GetAwaiter().GetResult();

            await SeedAsync(context);
        }
        private static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedUsersAsync(context);
        }
        private static async Task SeedStravaProfilesAsync(ApplicationDbContext context)
        {
            if (!await context.StravaProfiles.AnyAsync())
            {
                await context.StravaProfiles.AddRangeAsync(InitialData.StravaProfiles);
                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            if (!await context.Users.AnyAsync())
            {
                await context.Users.AddRangeAsync(InitialData.Users);
                await context.SaveChangesAsync();
            }
        }

       
    }
}
