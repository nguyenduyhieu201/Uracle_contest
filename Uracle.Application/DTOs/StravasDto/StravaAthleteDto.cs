using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.StravasDto
{
    public class StravaAthleteDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }


        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("firstname")]
        public string? Firstname { get; set; }

        [JsonPropertyName("lastname")]
        public string? Lastname { get; set; }
    }
}
