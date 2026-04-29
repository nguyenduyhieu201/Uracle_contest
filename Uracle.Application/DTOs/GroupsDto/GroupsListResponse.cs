using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.GroupsDto
{
    // Application/DTOs/GroupDtos.cs
    public record GroupResponse(
        string Id,
        string Name,
        string Description,
        bool Private,
        int MemberCount
    );

    public record GroupsListResponse(
        IEnumerable<GroupResponse> MyGroups,
        IEnumerable<GroupResponse> OtherGroups
    );
}