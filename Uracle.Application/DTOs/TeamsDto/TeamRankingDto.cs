using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.TeamsDto
{
    public class TeamRankingDto
    {
        public double TotalDistance { get; set; }
        public double AveragePace { get; set; }
        public double TotalTracklog { get; set; }
        public TeamRankingDto(double TotalDistance, double AveragePace, double TotalTracklog )
        {
            this.TotalDistance = TotalDistance;
            this.AveragePace = AveragePace;
            this.TotalTracklog = TotalTracklog;
        }
    }

}
