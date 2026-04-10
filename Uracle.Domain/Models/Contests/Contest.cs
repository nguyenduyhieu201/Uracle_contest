namespace Uracle.Domain.Models.Contests
{
    public class Contest : Entity<string>
    {
        public string Name { get; set; } = string.Empty;

        [ForeignKey("Group")]
        public string GroupId { get; set; } = string.Empty;

        [ForeignKey("User")]
        public string CreatedById { get; set; } = string.Empty; // user Id
        public virtual User CreatedBy { set; get; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public int NumberOfParticipants { get; set; }

        public int NumberOfTeams { get; set; }

        public string? Detail { get; set; }

        public ContestType ContestType { get; set; } = ContestType.Individual;

        public ActivityType ActivityType { get; set; } = ActivityType.Run;

        public double? MinPace { get; set; }

        public double? MaxPace { get; set; }

        public double? MinDistance { get; set; }

        public virtual ICollection<IndividualContestActivity> IndividualContestActivities { get; set; }
            = new List<IndividualContestActivity>();

        public virtual ICollection<TeamMemberActivity> TeamMemberActivities { get; set; }
            = new List<TeamMemberActivity>();   

        public virtual ICollection<Team> Teams { get; set; }
            = new List<Team>();

        // User tham gia contest (N–N)
        public virtual ICollection<ContestUser> ContestUsers { get; set; }
            = new List<ContestUser>();

        // Constructor private cho factory method
        private Contest(
            string id,
            string groupId,
            string createdById,
            string name,
            string? detail,
            DateTime startAt,
            DateTime endAt,
            ContestType contestType,
            ActivityType activityType,
            double? minPace,
            double? maxPace,
            double? minDistance)
        {
            Id = id;
            GroupId = groupId;
            CreatedById = createdById;
            Name = name;
            Detail = detail;
            StartAt = startAt;
            EndAt = endAt;
            ContestType = contestType;
            ActivityType = activityType;
            MinPace = minPace;
            MaxPace = maxPace;
            MinDistance = minDistance;
        }

        public static Contest Create(
            string groupId,
            string createdBy,
            string name,
            string? detail,
            DateTime startAt,
            DateTime endAt,
            ContestType contestType,
            ActivityType activityType,
            double? minPace,
            double? maxPace,
            double? minDistance)
        {
            return new Contest(
                Guid.NewGuid().ToString(),  // id
                groupId,
                createdBy,
                name,
                detail,
                startAt,
                endAt,
                contestType,
                activityType,
                minPace,
                maxPace,
                minDistance);
        }

        public void Update (
                string? name,
                DateTime? startAt,
                DateTime? endAt,
                ContestType? contestType
                // add more optional params if you need to update other fields
                )
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name.Trim();
            }
            if (startAt.HasValue)
            {
                StartAt = startAt.Value;
            }
            if (endAt.HasValue)
            {
                EndAt = endAt.Value;
            }
            if (contestType.HasValue)
            {
                ContestType = contestType.Value;
            }
        }

        public bool HasStarted(DateTime now) => StartAt <= now;
        public void AddTeam(Team team)
        {
            Teams.Add(team);
            NumberOfTeams++;
        }

        public void IncreaseParticipants(int count = 1)
        {
            NumberOfParticipants += count;
        }

        public void DecreaseParticipants(int count = 1)
        {
            NumberOfParticipants -= count;
        }

        //public void AddParticipant(User user)
        //{
        //    if (_participantIds.Contains(participantId))
        //        return;
        //    _participantIds.Add(participantId);
        //    NumberOfParticipants++;
        //    UpdatedAt = DateTime.UtcNow;
        //}
    }
}