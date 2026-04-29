using Uracle.Application.DTOs.StravasDto;

namespace Uracle.Application.DTOs.UsersDto
{
    public class UserProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public long? StravaId { get; set; }
        public StravaProfileDto? StravaProfile { get; set; }
        public bool? MustChangePassword { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PublicUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public StravaProfileDto? StravaProfile { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class WorkoutActivityDto
    {
        public string Id { get; set; } = string.Empty;
        public double Distance { get; set; }
        public int MovingTime { get; set; }
        public string WorkoutType { get; set; } = string.Empty;
        public double? Pace { get; set; }
        public DateTime StartDate { get; set; }
        public long? StravaActivityId { get; set; }
    }

    public class UpdateUserProfileRequest
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
    }
}
