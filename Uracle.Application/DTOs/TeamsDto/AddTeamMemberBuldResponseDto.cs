using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.TeamsDto
{
    public sealed class AddTeamMembersBulkResponse
    {
        public List<string> Successful { get; init; } = new List<string>();
        public List<string> Failed { get; init; } = new List<string>();
        public int Total => Successful.Count + Failed.Count;
    }
}
