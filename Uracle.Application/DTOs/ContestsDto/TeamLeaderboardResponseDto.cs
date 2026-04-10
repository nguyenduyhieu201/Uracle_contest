using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.ContestsDto
{
    public sealed record TeamLeaderboardResponseDto(
         string ContestId,
         string Metric,
         DateTime CachedAt,
         List<TeamLeaderboardItemDto> Teams
     );
}
