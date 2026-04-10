
namespace Uracle.Infrastructure.Options
{
    public class AuthCookieOptions
    {
        public AccessCookieOptions AccessToken { get; set; } = new();
        public RefreshCookieOptions RefreshToken { get; set; } = new();
        public bool Secure { get; set; }
        public string SameSiteAccess { get; set; } = string.Empty;
        public string SameSiteRefresh { get; set; } = string.Empty;
    }

}
