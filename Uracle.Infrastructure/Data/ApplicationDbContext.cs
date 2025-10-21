using Uracle.Domain.Models;
using Uracle.Domain.ValueObjects;

namespace Uracle.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IAppcationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                 : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<StravaProfile> StravaProfiles => Set<StravaProfile>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id)
                    .HasConversion(
                        id => id.Value,
                        value => UserId.Of(value))
                    .ValueGeneratedNever();
            });
        }
    }
}
