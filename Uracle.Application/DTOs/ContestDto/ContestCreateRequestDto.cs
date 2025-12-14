using Uracle.Domain.Models.Contests;

namespace Uracle.Application.DTOs.ContestDto
{
    public class ContestCreateRequestDto
    {
        public string UserId { get; set; } = string.Empty;      // nếu cần truyền từ client, còn không có thể bỏ
        public string GroupId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;

        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }

        public ContestType ContestType { get; set; }
        public ActivityType ActivityType { get; set; }

        public double? MinPace { get; set; }
        public double? MaxPace { get; set; }
        public double? MinDistance { get; set; }
        
    }
}