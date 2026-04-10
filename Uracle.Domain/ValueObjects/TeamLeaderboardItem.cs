using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Domain.ValueObjects
{
    public sealed record TeamLeaderboardItem(
        string TeamId,
        string TeamName,
        double TotalDistance,
        double AveragePace,
        int TotalTracklog,
        double FastestPace,
        double MaxDistance,
        int TotalActivities,
        int MemberCount,
        int Rank,
        string? Badge
    );
}
