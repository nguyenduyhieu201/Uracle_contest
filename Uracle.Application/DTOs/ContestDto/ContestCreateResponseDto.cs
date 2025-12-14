using System;
using Uracle.Domain.Models.Contests;

namespace Uracle.Application.DTOs.ContestDto
{
    public class ContestCreateResponseDto
    {
        public string ContestId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string? Detail { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public ContestType ContestType { get; set; }
        public ActivityType ActivityType { get; set; }

        public double? MinPace { get; set; }
        public double? MaxPace { get; set; }
        public double? MinDistance { get; set; }
    }
}