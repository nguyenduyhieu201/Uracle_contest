using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.ContestsDto
{
    public sealed record IndividualContestActivityDto(
        string Id,
        string ContestId,
        string UserId,
        DateTime StartDate,
        double Distance,
        double? Pace
    );
}
