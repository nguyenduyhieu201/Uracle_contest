using System.Text.RegularExpressions;

namespace Uracle.Application.Commands.UsersCommand
{
    public record UserRegisterCommand(UserRegisterDTO registerDto) : ICommand<Result<RegisterResponseDto>>;
    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        public UserRegisterCommandValidator()
        {
            RuleFor(x => x.registerDto.Username).NotEmpty().WithMessage("Username is required");
            RuleFor(x => x.registerDto.Username).MinimumLength(3).WithMessage("Username must be at least 3 characters.");
            RuleFor(x => x.registerDto.Password)
                        .NotEmpty()
                        .WithMessage("Password is required.")
                        .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\w\\s])\\S{6,64}$")
                        .WithMessage("Password must be 6-64 chars, include upper, lower, digit, special and no spaces.");
            RuleFor(x => x.registerDto.Email).NotEmpty().WithMessage("Email is required")
                                            .EmailAddress().WithMessage("Email must match with email format");
        }
    }
    public class UserRegisterCommandHandler : ICommandHandler<UserRegisterCommand, Result<RegisterResponseDto>> 
    {
        public IUserRepository _userRepository;
        public IPasswordHasher _passwordHasher;
        public UserRegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<Result<RegisterResponseDto>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            string validationError = await ValidateNewUserAsync(request.registerDto, cancellationToken);
            if (!string.IsNullOrEmpty(validationError))
            {
                return Result<RegisterResponseDto>.Fail(validationError);
            }

            var hashPassword = _passwordHasher.Hash(request.registerDto.Password);

            var user = User.Create(request.registerDto.Username, hashPassword, request.registerDto.Email);
            await _userRepository.AddUserAsync(user, cancellationToken);
            var registerDTO = new RegisterResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username
            };
            // Here you would normally create the user and save to the database
            return Result<RegisterResponseDto>.Success(registerDTO);
        }

        private async Task<string> ValidateNewUserAsync(UserRegisterDTO dto, CancellationToken cancellationToken)
        {
            var username = dto.Username?.Trim() ?? string.Empty;
            var email = dto.Email?.Trim() ?? string.Empty;
            var password = dto.Password ?? string.Empty;

            // Uniqueness
            if (await _userRepository.GetByUserNameAsync(username, cancellationToken) is not null)
                return "Username already exists.";

            // Nếu đã có GetByEmailAsync trong repo, bật kiểm tra này
             if (await _userRepository.GetByEmailAsync(email, cancellationToken) is not null)
                return "Email already exists.";

            return "";
        }

    }
}
