using Microsoft.AspNetCore.Identity;
using Uracle.Application.Abstractions.Services;

namespace Uracle.Application.Commands.UsersCommand
{
    public record UserForgetPasswordCommand(string Email) : ICommand<Result<bool>>;
    public class UserForgetPasswordCommandValidator : AbstractValidator<UserForgetPasswordCommand>
    {
        public UserForgetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .MaximumLength(256);
        }
    }
    public class UserForgetPasswordCommandHandler : ICommandHandler<UserForgetPasswordCommand, Result<bool>>
    {
        private IUserRepository _userRepository;
        private IJwtService _jwtService;
        private IEmailService _emailService;
        private IPasswordResetTokenRepository _passwordResetTokenRepository;
        private IPasswordHasher _passwordHasher;

        public UserForgetPasswordCommandHandler(IUserRepository userRepository, IPasswordResetTokenRepository passwordResetTokenRepository, IJwtService jwtService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _jwtService = jwtService;
            _emailService = emailService;
        }
        public async Task<Result<bool>> Handle(UserForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user =  await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                return Result<bool>.Fail("Email not found", ErrorCode.NotFound);
            }
            var isSuccess = await _passwordResetTokenRepository.RevokeValidTokensForUserAsync(user.Id, cancellationToken);
            if (!isSuccess)
            {
                return Result<bool>.Fail("Failed to revoke existing tokens", ErrorCode.InternalError);
            }
            var token = _jwtService.GeneratePasswordResetToken();
            var expiresAt = DateTime.UtcNow.AddHours(24); // Hết hạn sau 24h
            await _userRepository.UpdateResetTokenAsync(user.Id, token, expiresAt, cancellationToken);

            var resetLink = $"http://localhost:5012/reset-password?token={token}";
            await _emailService.SendPasswordResetEmailAsync(user.Email, user.Username, resetLink);

            return Result<bool>.Success(true);
            // Here you would typically generate a password reset token and send it via email.
        }
    }
}
