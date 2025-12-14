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
    public class IndividualContestActivity : Entity<string>
    {
        [ForeignKey("User")]

        public string UserId { get; set; }

        public string ContestId { get; set; }

        public int StravaUserId { get; set; }

        public double Distance { get; set; }

        public int MovingTime { get; set; }

        public string WorkoutType { get; set; } = null!;

        public double? Pace { get; set; }

        public DateTime StartDate { get; set; }

        public string WorkoutActivityId { get; set; }

        public long? StravaActivityId { get; set; }

        public virtual Contest Contest { get; set; } = null!;

        public virtual User User { get; set; } = null!;

        public virtual WorkoutActivity WorkoutActivity { get; set; } = null!;
    }

}
