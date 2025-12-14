using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.GroupMembers;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IGroupMemberRepository
    {
        public Task<List<GroupMember>> FindByUserId (string userId, CancellationToken cancellationToken);
    }
}
