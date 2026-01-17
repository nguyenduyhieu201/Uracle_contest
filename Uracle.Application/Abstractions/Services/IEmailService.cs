namespace Uracle.Application.Abstractions.Services
{
    public interface IEmailService
    {
        public Task SendPasswordResetEmailAsync(string email, string firstName, string resetLink);

    }
}
