namespace Uracle.Infrastructure.Options
{
    public class StravaOptions
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { set; get; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string Scope { get; set; } = "activity:read_all";
    }
}
