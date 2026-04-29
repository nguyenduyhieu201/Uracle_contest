using Uracle.Domain.Enums;

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
            await SeedGroupsAsync(context);
            await SeedGroupMembersAsync(context);
            await SeedContestsAsync(context);
            await SeedWorkoutActivitiesAsync(context);
            await SeedContestUsersAsync(context);
            await SeedIndividualContestActivitiesAsync(context);  // THÊM DÒNG NÀY
            await SeedTeamsAsync(context);
            await SeedTeamMembersAsync(context);
            await SeedTeamMemberActivitiesAsync(context);
        }
        private static async Task SeedIndividualContestActivitiesAsync(ApplicationDbContext context)
        {
            if (!await context.IndividualContestActivities.AnyAsync())
            {
                var contests = await context.Contests.ToListAsync();
                var individualContest = contests.First(c => c.ContestType == ContestType.Individual);

                await context.IndividualContestActivities.AddRangeAsync(InitialData.CreateIndividualContestActivities(individualContest.Id));
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


        private static async Task SeedGroupsAsync(ApplicationDbContext context)
        {
            if (!await context.Groups.AnyAsync())
            {
                await context.Groups.AddRangeAsync(InitialData.Groups);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedGroupMembersAsync(ApplicationDbContext context)
        {
            if (!await context.GroupsMembers.AnyAsync())
            {
                await context.GroupsMembers.AddRangeAsync(InitialData.GroupMembers);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedContestsAsync(ApplicationDbContext context)
        {
            if (!await context.Contests.AnyAsync())
            {
                await context.Contests.AddRangeAsync(InitialData.Contests);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedWorkoutActivitiesAsync(ApplicationDbContext context)
        {
            if (!await context.WorkoutActivities.AnyAsync())
            {
                await context.WorkoutActivities.AddRangeAsync(InitialData.WorkoutActivities);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedContestUsersAsync(ApplicationDbContext context)
        {
            if (!await context.ContestUsers.AnyAsync())
            {
                var contests = await context.Contests.ToListAsync();
                var individualContest = contests.First(c => c.ContestType == ContestType.Individual);
                var teamContest = contests.First(c => c.ContestType == ContestType.Team);

                await context.ContestUsers.AddRangeAsync(InitialData.CreateContestUsers(individualContest.Id, teamContest.Id));
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTeamsAsync(ApplicationDbContext context)
        {
            if (!await context.Teams.AnyAsync())
            {
                var contests = await context.Contests.ToListAsync();
                var teamContest = contests.First(c => c.ContestType == ContestType.Team);

                await context.Teams.AddRangeAsync(InitialData.CreateTeams(teamContest.Id));
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTeamMembersAsync(ApplicationDbContext context)
        {
            if (!await context.TeamMembers.AnyAsync())
            {
                var teams = await context.Teams.ToListAsync();
                var alphaTeam = teams.First(t => t.Name == "Alpha Runners");
                var betaTeam = teams.First(t => t.Name == "Beta Sprinters");

                await context.TeamMembers.AddRangeAsync(InitialData.CreateTeamMembers(alphaTeam.Id, betaTeam.Id));
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTeamMemberActivitiesAsync(ApplicationDbContext context)
        {
            if (!await context.TeamMemberActivities.AnyAsync())
            {
                var teams = await context.Teams.ToListAsync();
                var contests = await context.Contests.ToListAsync();
                var alphaTeam = teams.First(t => t.Name == "Alpha Runners");
                var betaTeam = teams.First(t => t.Name == "Beta Sprinters");
                var teamContest = contests.First(c => c.ContestType == ContestType.Team);

                await context.TeamMemberActivities.AddRangeAsync(InitialData.CreateTeamMemberActivities(alphaTeam.Id, betaTeam.Id, teamContest.Id));
                await context.SaveChangesAsync();
            }
        }
    }
}