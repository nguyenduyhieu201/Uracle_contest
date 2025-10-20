using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Domain.Models
{
    public class StravaProfile
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
    }

    public class User
    {

        public string Id { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }

        public long? StravaId { get; set; }
        public StravaProfile? StravaProfile { get; set; }

        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public long? ExpiresAt { get; set; }

        public string? JwtRefreshToken { get; set; }

        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public bool? MustChangePassword { get; set; }

        public List<string>? GroupIds { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
