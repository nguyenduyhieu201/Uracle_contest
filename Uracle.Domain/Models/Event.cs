using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Domain.Models
{
    public class Event
    {
        public Guid Id { get; set; }

        public string AspectType { get; set; } = null!;

        public int EventTime { get; set; }

        public long ObjectId { get; set; }

        public string ObjectType { get; set; } = null!;

        public int OwnerId { get; set; }

        public int SubscriptionId { get; set; }

        public string? Updates { get; set; }

        public DateTime ReceivedAt { get; set; }

        public string Status { get; set; } = null!;

        public int? Attempts { get; set; }

        public string? Error { get; set; }
    }

}
