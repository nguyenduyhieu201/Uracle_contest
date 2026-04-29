using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Domain.Models
{
    public class WebhookEvent
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(50)]
        public string AspectType { get; set; } = string.Empty; // create, update, delete

        [Required]
        [MaxLength(50)]
        public string ObjectType { get; set; } = string.Empty; // activity, athlete

        [Required]
        public long ObjectId { get; set; } // Strava activity/athlete ID

        [Required]
        public long OwnerId { get; set; } // Strava user ID

        [Required]
        public long SubscriptionId { get; set; }

        [Required]
        public long EventTime { get; set; } // Unix timestamp

        public string? UpdatesJson { get; set; } // JSON string for updates

        [Required]
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Successful, Failed

        public int Attempts { get; set; } = 0;

        public string? ErrorMessage { get; set; }

        [NotMapped]
        public Dictionary<string, string>? Updates
        {
            get => string.IsNullOrEmpty(UpdatesJson)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(UpdatesJson);
            set => UpdatesJson = value == null
                ? null
                : System.Text.Json.JsonSerializer.Serialize(value);
        }
    }
}
