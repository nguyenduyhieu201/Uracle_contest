using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Infrastructure.Options
{
    public class AccessCookieOptions
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int ExpiresInHours { get; set; } = -1;
    }
}
