using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Infrastructure.Options
{
    public class RefreshCookieOptions
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int ExpiresInDays { get; set; } = -1;
    }
}
