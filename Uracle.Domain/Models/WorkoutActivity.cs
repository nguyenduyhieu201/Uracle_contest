using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Domain.Models
{
    public class WorkoutActivity
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public int StravaUserId { get; set; }

        public double Distance { get; set; }

        public int MovingTime { get; set; }

        public string WorkoutType { get; set; } = null!;

        public double? Pace { get; set; }

        public DateTime StartDate { get; set; }

        public long? StravaActivityId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<IndividualContestActivity> IndividualContestActivities { get; set; }

        public ICollection<TeamMemberActivity?> TeamMemberActivities { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
