using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.Queries.ContestsQuery;

namespace Uracle.Application.DTOs.TeamsDto
{
    public record TeamMemberDto(
        string UserId,
        DateTime JoinedAt,
        string Name,
        string Username,
        StravaProfileDto StravaProfile
    );
}
