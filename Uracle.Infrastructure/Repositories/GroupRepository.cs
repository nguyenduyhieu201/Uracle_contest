
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

        public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
        {
            await _context.Groups.AddAsync(group, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> CanManageContestAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.GroupsMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.Role == UserRole.admin, cancellationToken);
        }

        public async Task<List<GroupMember>> FindByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.GroupsMembers.Where(m => m.UserId == userId).ToListAsync(cancellationToken);
        }

        public async Task<List<Group>> GetAllAsync()
        {
            return await _context.Groups.ToListAsync();
        }

        public async Task<List<User>> GetByGroupIdAsync(string groupId, CancellationToken cancellationToken)
        {
            return await _context.GroupsMembers
                .Where(m => m.GroupId == groupId)
                .Join(_context.Users, gm => gm.UserId, u => u.Id, (gm, u) => u)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsUserAdminInGroup(string userId, string groupId, CancellationToken cancellationToken)
        {
            return await _context.GroupsMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId && m.Role == UserRole.admin, cancellationToken);
        }

        // Group CRUD
        public async Task<Group?> GetByIdAsync(string groupId, CancellationToken cancellationToken = default)
        {
            return await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);
        }

        public async Task UpdateAsync(Group group, CancellationToken cancellationToken = default)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(string groupId, CancellationToken cancellationToken = default)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);
            if (group == null) return false;
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Member management
        public async Task<List<GroupMember>> GetGroupMembersAsync(string groupId, int page, int limit, CancellationToken cancellationToken = default)
        {
            return await _context.GroupsMembers
                .Include(m => m.User)
                .Where(m => m.GroupId == groupId)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        public async Task<GroupMember> AddGroupMemberAsync(string groupId, string userId, UserRole role, CancellationToken cancellationToken = default)
        {
            var member = new GroupMember
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = groupId,
                UserId = userId,
                Role = role,
                JoinedAt = DateTime.UtcNow
            };
            await _context.GroupsMembers.AddAsync(member, cancellationToken);
            var group = await _context.Groups.FirstOrDefaultAsync(group => group.Id == groupId);
            if (group != null) group.MemberCount += 1;
            await _context.SaveChangesAsync(cancellationToken);            
            return member;
        }

        public async Task<bool> RemoveMemberAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            var member = await _context.GroupsMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken);
            if (member == null) return false;
            _context.GroupsMembers.Remove(member);
            var group = await _context.Groups.FirstOrDefaultAsync(group => group.Id == groupId);
            if (group != null) group.MemberCount += 1;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateMemberRoleAsync(string groupId, string userId, UserRole role, CancellationToken cancellationToken = default)
        {
            var member = await _context.GroupsMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken);
            if (member == null) return false;
            member.Role = role;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<UserRole?> GetUserRoleAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            var member = await _context.GroupsMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken);
            return member?.Role;
        }

        public async Task<bool> IsMemberAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.GroupsMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken);
        }

        // Join requests
        public async Task<bool> HasPendingJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.JoinRequests
                .AnyAsync(r => r.GroupId == groupId && r.UserId == userId && r.Status == "pending", cancellationToken);
        }

        public async Task<bool> CreateJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            var request = new JoinRequest
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = groupId,
                UserId = userId,
                Status = "pending",
                RequestedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow
            };
            await _context.JoinRequests.AddAsync(request, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<JoinRequest>> GetPendingJoinRequestsAsync(string groupId, CancellationToken cancellationToken = default)
        {
            return await _context.JoinRequests
                .Include(r => r.User)
                .Where(r => r.GroupId == groupId && r.Status == "pending")
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ApproveJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            var request = await _context.JoinRequests
                .FirstOrDefaultAsync(r => r.GroupId == groupId && r.UserId == userId && r.Status == "pending", cancellationToken);
            if (request == null) return false;
            request.Status = "approved";
            request.ProcessedAt = DateTime.UtcNow;
            // Add as member
            var alreadyMember = await IsMemberAsync(groupId, userId, cancellationToken);
            if (!alreadyMember)
            {
                await AddGroupMemberAsync(groupId, userId, UserRole.member, cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RejectJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            var request = await _context.JoinRequests
                .FirstOrDefaultAsync(r => r.GroupId == groupId && r.UserId == userId && r.Status == "pending", cancellationToken);
            if (request == null) return false;
            request.Status = "rejected";
            request.ProcessedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RevokeJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default)
        {
            var request = await _context.JoinRequests
                .FirstOrDefaultAsync(r => r.GroupId == groupId && r.UserId == userId && r.Status == "pending", cancellationToken);
            if (request == null) return false;
            _context.JoinRequests.Remove(request);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
