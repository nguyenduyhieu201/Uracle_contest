using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.TeamsDto
{
    public class TeamStatsDto
    {
        public string TeamId { get; set; }
        public string TeamName { get; set; }
        public string ContestId { get; set; }
        public TeamStatsDetailDto Stats { get; set; }
        public TeamRankingDto Ranking { get; set; }
        public DateTime CachedAt { get; set; }
    }
}
