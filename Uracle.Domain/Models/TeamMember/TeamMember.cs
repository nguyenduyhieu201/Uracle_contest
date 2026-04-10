using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Abstractions;

namespace Uracle.Domain.Models.TeamMember
{
    public class TeamMember
    {
        public string TeamId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.member; // possible values: member, admin
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
        public Team Team { set; get; }

        public TeamMember(
            string teamId,
            string userId,
            UserRole role,
            DateTime joinedAt)
        {
            TeamId = teamId;
            UserId = userId;
            Role = role;
            JoinedAt = joinedAt;
        }
    }
}
