namespace Uracle.Infrastructure.Options
{
    public class RefreshCookieOptions
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public int ExpiresInDays { get; set; } = -1;
    }
}
