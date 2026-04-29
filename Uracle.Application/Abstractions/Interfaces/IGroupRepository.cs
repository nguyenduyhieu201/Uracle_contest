using Uracle.Domain.Enums;
using Uracle.Domain.Models.GroupMembers;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IGroupRepository
    {
        // Existing
        Task<bool> CanManageContestAsync(string groupId, string userId, CancellationToken cancellationToken = default);
        Task<List<GroupMember>> FindByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<List<User>> GetByGroupIdAsync(string groupId, CancellationToken cancellationToken);
        Task<List<Group>> GetAllAsync();
        Task<bool> IsUserAdminInGroup(string userId, string groupId, CancellationToken cancellationToken);
        Task AddAsync(Group group, CancellationToken cancellationToken = default);

        // Group CRUD
        Task<Group?> GetByIdAsync(string groupId, CancellationToken cancellationToken = default);
        Task UpdateAsync(Group group, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string groupId, CancellationToken cancellationToken = default);

        // Member management
        Task<List<GroupMember>> GetGroupMembersAsync(string groupId, int page, int limit, CancellationToken cancellationToken = default);
        Task<GroupMember> AddGroupMemberAsync(string groupId, string userId, UserRole role, CancellationToken cancellationToken = default);
        Task<bool> RemoveMemberAsync(string groupId, string userId, CancellationToken cancellationToken = default);
        Task<bool> UpdateMemberRoleAsync(string groupId, string userId, UserRole role, CancellationToken cancellationToken = default);
        Task<UserRole?> GetUserRoleAsync(string groupId, string userId, CancellationToken cancellationToken = default);
        Task<bool> IsMemberAsync(string groupId, string userId, CancellationToken cancellationToken = default);

        // Join requests
        Task<bool> HasPendingJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default);
        Task<bool> CreateJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default);
        Task<List<JoinRequest>> GetPendingJoinRequestsAsync(string groupId, CancellationToken cancellationToken = default);
        Task<bool> ApproveJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default);
        Task<bool> RejectJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default);
        Task<bool> RevokeJoinRequestAsync(string groupId, string userId, CancellationToken cancellationToken = default);
    }
}
