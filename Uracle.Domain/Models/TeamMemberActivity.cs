using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Abstractions;
using Uracle.Domain.Models.Contests;

namespace Uracle.Domain.Models
{
    public class TeamMemberActivity : Entity<string>
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("Team")]
        public string TeamId { get; set; }
        [ForeignKey("Contest")]
        public string ContestId { get; set; }
        public int StravaUserId { get; set; }
        public double Distance { get; set; }
        public int MovingTime { get; set; }
        public string WorkoutType { get; set; } = null!;
        public double? Pace { get; set; }
        public DateTime StartDate { get; set; }
        public long? StravaActivityId { get; set; }

        [ForeignKey("WorkoutActivity")]
        public string WorkoutActivityId { get; set; }
        public Contest Contest { get; set; } = null!;
        public Team Team { get; set; } = null!;
        public User User { get; set; } = null!;
        public WorkoutActivity WorkoutActivity { get; set; } = null!;

    }
}
