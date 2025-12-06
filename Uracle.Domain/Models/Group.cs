using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Abstractions;

namespace Uracle.Domain.Models
{
    public class Group : Entity<string>
    {
        public string Name { set; get; } = string.Empty;
        public string Description { set; get; } = string.Empty;
        public bool IsPrivate { set; get; } = false;
        public int MemberCount { set; get; } = 0;
        public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

    }
}
