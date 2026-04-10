using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Abstractions;
using Uracle.Domain.Models.Contests;
using Uracle.Domain.Models.TeamMember;
using Uracle.Domain.ValueObjects;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TeamMem = Uracle.Domain.Models.TeamMember.TeamMember;

namespace Uracle.Domain.Models
{
    public class Team : Entity<string>
    {
        [ForeignKey("Group")]
        public string GroupId { get; set; }
        [ForeignKey("Contest")]
        public string ContestId { get; set; }
        public string Name { get; set; } = null!;

        public int NumberOfMembers { get; set; } = 0;
        public double AveragePace { get; set; } = 0.0;
        public double TotalDistance { get; set; }
        public int TotalTrackLog { get; set; } = 0;
        public double FastestPace { set; get; }
        public double MaxDistance { set; get; }

        public Contest Contest { get; set; } = null!;
        public Group Group { get; set; } = null!;
        public virtual ICollection<TeamMemberActivity> TeamMemberActivities { get; set; } = new List<TeamMemberActivity>();
        public virtual ICollection<TeamMem> TeamMembers { get; set; } = new List<TeamMem>();
        private Team(
            string groupId,
            string contestId,
            string name
            )
        {
            Id = Guid.NewGuid().ToString();
            GroupId = groupId;
            ContestId = contestId;
            Name = name;
            NumberOfMembers = 0;
            AveragePace = 0;
            TotalDistance = 0;
            TotalTrackLog = 0;
            FastestPace = 0;
            MaxDistance = 0;

        }
        public static Team Create(
            string groupId,
            string contestId,
            string name)
            => new(
                groupId: groupId,
                contestId: contestId,
                name: name
            );

        public Result<TeamMem> AddMember(string userId, DateTime nowUtc = default)
        {
            if (nowUtc == default)
                nowUtc = DateTime.UtcNow;
            if (TeamMembers.Any(m => m.UserId == userId))
                return Result<TeamMem>.Fail("User is already a team member", ErrorCode.BadRequest);
            var teamMember = new TeamMem(Id, userId, UserRole.member, nowUtc);
            TeamMembers.Add(teamMember);
            return Result<TeamMem>.Success(teamMember);
        }

        public Result<TeamMem> RemoveMember(string userId) 
        {
            var member = TeamMembers.FirstOrDefault(m => m.UserId == userId);
            if (member is null)
                return Result<TeamMem>.Fail("Member not found in team.",ErrorCode.NotFound);
            TeamMembers.Remove(member);
            return Result<TeamMem>.Success(member);
        }
    }
}
