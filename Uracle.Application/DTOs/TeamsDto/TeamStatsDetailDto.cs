using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.TeamsDto
{
    public class TeamStatsDetailDto
    {
        public string TeamName { set; get; }
        public double TotalDistance { get; set; }
        public double AveragePace { get; set; }
        public int TotalTracklog { get; set; }
        public double FastestPace { get; set; }
        public double MaxDistance { get; set; }
        public int TotalActivities { get; set; }
        public int MemberCount { get; set; }
    }
}
