

using Uracle.Domain.Enums;

namespace Uracle.Infrastructure.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private ApplicationDbContext _context;
        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CanManageContestAsync(
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

        public async Task<List<GroupMember>> FindByUserId(string userId, CancellationToken cancellationToken)
        {
            return await _context.GroupsMembers
                .Where(m => m.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<User>> GetByGroupIdAsync(string groupId, CancellationToken cancellationToken)
        {
            return await _context.GroupsMembers
                .Where(m => m.GroupId == groupId)
                .Join(_context.Users,
                      gm => gm.UserId,
                      u => u.Id,
                      (gm, u) => u)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUserAdminInGroup(string userId, string groupId, CancellationToken cancellationToken)
        {
            return await _context.GroupsMembers
                .AnyAsync(m => m.GroupId == groupId
                               && m.UserId == userId
                               && m.Role == UserRole.admin,
                          cancellationToken);
        }
    }
}
