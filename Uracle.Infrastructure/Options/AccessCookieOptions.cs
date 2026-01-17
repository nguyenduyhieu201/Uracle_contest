namespace Uracle.Infrastructure.Options
{
    public class AccessCookieOptions
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int ExpiresInHours { get; set; } = -1;
    }
}
