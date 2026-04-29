using Uracle.Domain.Models.Contests;
using Uracle.Domain.Models.GroupMembers;
using Uracle.Domain.Models.TeamMember;
using Uracle.Domain.ValueObjects;

namespace Uracle.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                 : base(options) { }
        public DbSet<WebhookEvent> WebhookEvents => Set<WebhookEvent>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<GroupMember> GroupsMembers => Set<GroupMember>();
        public DbSet<Contest> Contests => Set<Contest>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
        public DbSet<TeamMemberActivity> TeamMemberActivities => Set<TeamMemberActivity>();
        public DbSet<IndividualContestActivity> IndividualContestActivities => Set<IndividualContestActivity>();
        public DbSet<ContestUser> ContestUsers => Set<ContestUser>();
        public DbSet<WorkoutActivity> WorkoutActivities => Set<WorkoutActivity>();
        public DbSet<JoinRequest> JoinRequests => Set<JoinRequest>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsOne(u => u.StravaProfile);
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

            modelBuilder.Entity<IndividualContestActivity>()
                .HasOne(ica => ica.WorkoutActivity)
                .WithMany(wa => wa.IndividualContestActivities)
                .HasForeignKey(ica => ica.WorkoutActivityId)
                .OnDelete(DeleteBehavior.NoAction);   // hoặc Restrict
                                                      // WorkoutActivity -> TeamMemberActivity
            modelBuilder.Entity<TeamMemberActivity>()
                .HasOne(tma => tma.WorkoutActivity)
                .WithMany(wa => wa.TeamMemberActivities)
                .HasForeignKey(tma => tma.WorkoutActivityId)
                .OnDelete(DeleteBehavior.NoAction);   // ít nhất 1 trong 2 không cascade

            modelBuilder.Entity<TeamMemberActivity>()
                .HasOne(tma => tma.Team)
                .WithMany(t => t.TeamMemberActivities)
                .HasForeignKey(tma => tma.TeamId)
                .OnDelete(DeleteBehavior.NoAction);  // hoặc Restrict
                                                     // Team -> TeamMember
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.NoAction);  // ít nhất một trong hai phải không cascade

            modelBuilder.Entity<TeamMember>()
                .HasKey(x => new { x.TeamId, x.UserId });

            modelBuilder.Entity<TeamMember>(b =>
            {
                b.Property(x => x.Role)
                    .HasConversion<string>()        // lưu dưới dạng string
                    .HasMaxLength(20);
            });


            modelBuilder.Entity<Contest>(b =>
            {
                b.HasOne(c => c.CreatedBy)
                    .WithMany(u => u.CreatedContests)
                    .HasForeignKey(c => c.CreatedById)
                    .OnDelete(DeleteBehavior.NoAction); // hoặc Restrict
            });

            modelBuilder.Entity<WebhookEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AspectType).HasMaxLength(50);
                entity.Property(e => e.ObjectType).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.ReceivedAt);
                entity.HasIndex(e => e.ObjectId);
            });
        }
    }
}
