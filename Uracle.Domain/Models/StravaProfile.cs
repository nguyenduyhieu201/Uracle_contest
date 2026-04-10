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

        public void UpdateFromAthlete(int athleteId, string userName, string firstName, string lastName)
        {
            Id = athleteId;
            Username = userName ?? string.Empty;
            Firstname = firstName ?? string.Empty;
            Lastname = lastName ?? string.Empty;
        }
    }
}
