using System.Text.Json.Serialization;

namespace Uracle.Application.DTOs.StravasDto
{
    // Response from GET /api/v3/activities/{id}
    public class StravaActivityResponseDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        // Legacy field (still returned alongside sport_type)
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        // Preferred field for activity classification
        [JsonPropertyName("sport_type")]
        public string SportType { get; set; } = string.Empty;

        [JsonPropertyName("distance")]
        public double Distance { get; set; } // meters

        [JsonPropertyName("moving_time")]
        public int MovingTime { get; set; } // seconds

        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("average_speed")]
        public double AverageSpeed { get; set; } // m/s

        /// <summary>Resolves the best available activity type label.</summary>
        public string ResolvedType =>
            !string.IsNullOrWhiteSpace(SportType) ? SportType : Type;
    }
}
