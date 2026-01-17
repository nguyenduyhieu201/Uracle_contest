using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Domain.ValueObjects
{
    public sealed record TeamLeaderboardResult(
        string ContestId,
        TeamLeaderboardMetric Metric,
        DateTime CachedAt,
        List<TeamLeaderboardItem> Teams
    );

}
