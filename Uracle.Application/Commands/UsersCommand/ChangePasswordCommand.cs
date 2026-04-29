using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.Abstractions.Services;
using Uracle.Application.DTOs.UsersDto;

namespace Uracle.Application.Commands.UsersCommand
{
    public record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword,
        string UserId
    ) : ICommand<Result<Unit>>;
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("CurrentPassword is required");
            RuleFor(x => x.NewPassword)
                        .NotEmpty()
                        .WithMessage("Password is required.")
                        .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\w\\s])\\S{6,64}$")
                        .WithMessage("Password must be 6-64 chars, include upper, lower, digit, special and no spaces.");

        }
    }
    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, Result<Unit>>
    {
        private IUserRepository _userRepository;
        private IJwtService _jwtService;
        private IEmailService _emailService;
        private IPasswordResetTokenRepository _passwordResetTokenRepository;
        private IPasswordHasher _passwordHasher;
        public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordResetTokenRepository passwordResetTokenRepository, IJwtService jwtService, IEmailService emailService, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _passwordHasher = passwordHasher;

        }
        public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return Result<Unit>.Fail("User not found", ErrorCode.NotFound);
            }
            // Hash passwords
            var currentPasswordHash = _passwordHasher.Hash(request.CurrentPassword);
            var newPasswordHash = _passwordHasher.Hash(request.NewPassword);

            if (user.PasswordHash != currentPasswordHash)
            {
                return Result<Unit>.Fail("Current password is incorrect", ErrorCode.Unauthorized);
            }

            await _userRepository.UpdatePasswordAsync(user, newPasswordHash, cancellationToken);
            return Result<Unit>.Success(Unit.Value);    
        }
    }
}
