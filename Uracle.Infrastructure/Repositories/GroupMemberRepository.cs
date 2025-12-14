using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.GroupMembers;

namespace Uracle.Infrastructure.Repositories
{
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private ApplicationDbContext _context;
        public GroupMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupMember>> FindByUserId(string userId, CancellationToken cancellationToken)
        {
            return await _context.GroupsMembers
                .Where(m => m.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
