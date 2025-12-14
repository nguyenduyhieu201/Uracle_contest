using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.GroupMembers;

namespace Uracle.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private ApplicationDbContext _context;
        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CanCreateContestAsync(
            string groupId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.GroupsMembers
                .AnyAsync(m => m.GroupId == groupId
                               && m.UserId == userId
                               && m.Role == UserRole.admin,
                          cancellationToken);
        }
    }
}
