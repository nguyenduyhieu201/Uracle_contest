using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.IndividualContestActivityDto
{
    public sealed record IndividualContestLeaderboardRow(
        string UserId,
        double TotalDistance,
        int TotalTracklog,
        double AveragePace,
        double FastestPace,
        double MaxDistance
    );
}
