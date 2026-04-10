using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Abstractions;

namespace Uracle.Domain.Models.GroupMembers
{
    public class GroupMember : Entity<string>
    {
        public string UserId { set; get; } = string.Empty;
        public string GroupId { set; get; } = string.Empty;
        public UserRole Role { set; get; } = 0; // possible values: member, admin
        public DateTime JoinedAt { set; get; } = DateTime.UtcNow;

        public User User { set; get; } = null!;
        public Group Group { set; get; } = null!;
    }
}
