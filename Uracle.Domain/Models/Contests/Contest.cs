using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Uracle.Domain.Abstractions;

namespace Uracle.Domain.Models.Contests
{
    public class Contest : Entity<string>
    {
        public string Name { get; set; } = string.Empty;

        [ForeignKey("Group")]
        public string GroupId { get; set; } = string.Empty;

        [ForeignKey("User")]
        public string CreatedBy { get; set; } = string.Empty; // user Id

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

        public virtual ICollection<Team> TeamsNavigation { get; set; }
            = new List<Team>();

        public virtual ICollection<Team> Teams { get; set; }
            = new List<Team>();

        public virtual ICollection<User> Users { get; set; }
            = new List<User>();

        // Constructor private cho factory method
        private Contest(
            string id,
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
            Id = id;
            GroupId = groupId;
            CreatedBy = createdBy;
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
    }
}