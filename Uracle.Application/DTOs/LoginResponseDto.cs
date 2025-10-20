
namespace Uracle.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Username { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
