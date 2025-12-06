using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Abstractions;

namespace Uracle.Domain.Models
{
    public class TeamMember: Entity<string>
    {
        public string TeamId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = "member"; // possible values: member, admin
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
        //public Team Team { set; get; }
    }
}
