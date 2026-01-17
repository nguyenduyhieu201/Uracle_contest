using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models.Contests;

namespace Uracle.Domain.Models
{
    public class ContestUser : Entity<string>
    {
        public string ContestId { get; set; }
        public virtual Contest Contest { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public DateTime JoinedAt { get; set; }
        public ContestUser() { } // for EF
        private ContestUser(string id, string contestId, string userId, DateTime joinedAt)
        {
            Id = id;
            ContestId = contestId;
            UserId = userId;
            JoinedAt = joinedAt;
        }
        public static Result<ContestUser> Create(string contestId, string userId)
        {
            if (string.IsNullOrEmpty(contestId))
            {
                return Result<ContestUser>.Fail("ContestId cannot be empty", ErrorCode.BadRequest);
            }
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result<ContestUser>.Fail("UserId cannot be empty", ErrorCode.BadRequest);

            }
            var contestUser = new ContestUser(
                id: Guid.NewGuid().ToString(),
                contestId: contestId,
                userId: userId.Trim(),
                joinedAt: DateTime.UtcNow
            );
            return Result<ContestUser>.Success(contestUser);
        }
    }
}
