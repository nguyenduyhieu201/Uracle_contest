namespace Uracle.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendPasswordResetEmailAsync(string email, string firstName, string resetLink)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var useRealEmail = emailSettings.GetValue<bool>("UseRealEmail", false);

            if (useRealEmail)
            {
                await SendRealEmailAsync(email, firstName, resetLink);
            }
            else
            {
                //// Log mode for development/testing
                //    email, firstName, resetLink);

                // Simulate email sending delay
                await Task.Delay(100);
            }
        }

        private async Task SendRealEmailAsync(string email, string firstName, string resetLink)
        {
            try
            {
                // TODO: Implement real email sending using SMTP, SendGrid, etc.
                var smtpSettings = _configuration.GetSection("EmailSettings:Smtp");
                var smtpServer = smtpSettings["Server"];
                var smtpPort = smtpSettings.GetValue<int>("Port", 587);
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"];

                if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(smtpUsername))
                {
                    await SendPasswordResetEmailAsync(email, firstName, resetLink); // Recursive call with log mode
                    return;
                }

                // Example using System.Net.Mail (you can replace with SendGrid, MailKit, etc.)
                using var mailMessage = new System.Net.Mail.MailMessage
                {
                    From = new System.Net.Mail.MailAddress(fromEmail, fromName),
                    Subject = "Reset Your Password",
                    Body = GeneratePasswordResetEmailBody(firstName, resetLink),
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                using var smtpClient = new System.Net.Mail.SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword)
                };

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Fallback to log mode
                await SendPasswordResetEmailAsync(email, firstName, resetLink);
            }
        }
        private string GeneratePasswordResetEmailBody(string firstName, string resetLink)
        {
            return $@"
                <html>
                <body>
                    <h2>Hello {firstName},</h2>
                    <p>You have requested to reset your password.</p>
                    <p>Click the link below to reset your password:</p>
                    <p><a href='{resetLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Reset Password</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you didn't request this, please ignore this email.</p>
                    <p>Best regards,<br/>Your App Team</p>
                </body>
                </html>";
        }
    }
}
