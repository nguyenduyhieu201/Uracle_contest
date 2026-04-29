using Uracle.Domain.Models.GroupMembers;

namespace Uracle.Domain.Models
{
    public class Group : Entity<string>
    {
        public string Name { set; get; } = string.Empty;
        public string Description { set; get; } = string.Empty;
        public bool IsPrivate { set; get; } = false;
        public int MemberCount { set; get; } = 0;
        public virtual ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();
        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

        public bool CanUserView( bool isUserMember)
        {
            // Public groups can be viewed by anyone
            if (!IsPrivate)
                return true;

            // Private groups can only be viewed by members
            return isUserMember;
        }

        public static Group Create(string name, string description, bool isPrivate, Guid createdBy)
        {
            return new Group
            {
                Name = name.Trim(),
                Description = description?.Trim() ?? string.Empty,
                IsPrivate = isPrivate,
                MemberCount = 1

            };
        }

    }
}
