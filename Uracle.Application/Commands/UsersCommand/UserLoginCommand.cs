using SharedKernel.Domains;
using Uracle.Application.Abstractions.Security;

namespace Uracle.Application.Commands.UsersCommand
{
    public record UserLoginCommand(UserLoginDTO LoginDTO) : ICommand<Result<LoginResponseDto>>;
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {
        public UserLoginCommandValidator()
        {
            RuleFor(RuleFor => RuleFor.LoginDTO.Username).NotEmpty();
            RuleFor(x => x.LoginDTO.Password).NotEmpty().MinimumLength(6);
        }
    }

    public class UserLoginCommandHandler : ICommandHandler<UserLoginCommand, Result<LoginResponseDto>>
    {

        private IUserRepository _userRepository;
        private IPasswordHasher _passwordHasher;
        private IJWTService _jwtService;

        public UserLoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJWTService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }
        public async Task<Result<LoginResponseDto>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var user = await ValidateAsync(request.LoginDTO.Username, request.LoginDTO.Password, cancellationToken);
            if (user == null)
            {
                return Result<LoginResponseDto>.Fail("Invalid username or password.");
            }
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var responseDto = new LoginResponseDto
            {
                Username = user.Username,
                JwtToken = token,
                RefreshToken = refreshToken
            };
            return Result<LoginResponseDto>.Success(responseDto);
        }

        private async Task<User?> ValidateAsync(string username, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUserNameAsync(username, cancellationToken);
            if (user == null)
            {
                return null;
            }

            if (!_passwordHasher.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }
    }
}
