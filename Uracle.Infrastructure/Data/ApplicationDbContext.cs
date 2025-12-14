using Uracle.Domain.Models.Contests;
using Uracle.Domain.Models.GroupMembers;
using Uracle.Domain.ValueObjects;

namespace Uracle.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IAppcationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                 : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<StravaProfile> StravaProfiles => Set<StravaProfile>();
        public DbSet<GroupMember> GroupsMembers => Set<GroupMember>();
        public DbSet<Contest> Contests => Set<Contest>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Id)
                    .HasMaxLength(36)       // nếu dùng Guid.ToString() có dấu gạch
                    .IsRequired()
                    .ValueGeneratedNever(); // app tự set Id, không để DB sinh
            });

            modelBuilder.Entity<GroupMember>(b =>
            {
                b.Property(x => x.Role)
                    .HasConversion<string>()        // lưu dưới dạng string
                    .HasMaxLength(20);
            });
        }
    }
}
