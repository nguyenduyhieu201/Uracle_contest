using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.GroupMembers;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IGroupRepository
    {
        public Task<bool> CanManageContestAsync(string groupId, string userId, CancellationToken cancellationToken = default);
        public Task<List<GroupMember>> FindByUserId(string userId, CancellationToken cancellationToken);
        Task<List<User>> GetByGroupIdAsync(string groupId, CancellationToken cancellationToken);
        public Task<bool> IsUserAdminInGroup(string userId, string groupId, CancellationToken cancellationToken);
    }
}
