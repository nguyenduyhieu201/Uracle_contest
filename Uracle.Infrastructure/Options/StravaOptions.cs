using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Infrastructure.Options
{
    public class StravaOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string Scope { get; set; } = "activity:read_all";
    }
}
