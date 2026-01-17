using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.TeamsDto
{
    public sealed class AddTeamMembersBulkRequest
    {
        public List<string> UserIds { get; init; } = new List<string>();
    }
}
