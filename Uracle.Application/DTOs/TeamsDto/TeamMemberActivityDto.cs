using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.TeamsDto
{
    public class TeamMemberActivityDto
    {
        public string Id { get; set; }
        public string TeamId { get; set; }
        public string UserId { get; set; }
        public string ContestId { get; set; }
        public double Distance { get; set; }
        public double Pace { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ActivityName { get; set; }
        public string ActivityType { get; set; }
        public long StravaActivityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
