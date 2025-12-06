using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Abstractions;

namespace Uracle.Domain.Models
{
    public class JoinRequest : Entity<string>
    {
        [ForeignKey("User")]
        public string UserId { set; get; } = string.Empty;
        public User User { get; set; } = null!; 
        [ForeignKey("Group")]   
        public string GroupId { set; get; } = string.Empty;
        public Group Group { set; get; }
        public DateTime RequestedAt { set; get; } = DateTime.UtcNow;
        public string Status { set; get; } = "pending"; // possible values: pending, approved, rejected
        public DateTime ProcessedAt { set; get; } = DateTime.UtcNow;
    }
}
