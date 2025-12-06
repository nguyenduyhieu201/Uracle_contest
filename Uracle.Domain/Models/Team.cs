using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Abstractions;

namespace Uracle.Domain.Models
{
    public class Team : Entity<string>
    {
        [ForeignKey("Group")]
        public Guid GroupId { get; set; }
        [ForeignKey("Contest")]
        public Guid ContestId { get; set; }
        public string Name { get; set; } = null!;

        public int NumberOfMembers { get; set; } = 0;
        public double AveragePace { get; set; } = 0.0;
        public double TotalDistance { get; set; }
        public int TotalTrackLog { get; set; } = 0;
        public double FastestPace { set; get; }
        public double MaxDistance { set; get; }

        //public Contest Contest { get; set; } = null!;
        public Group Group { get; set; } = null!;
    }
}
