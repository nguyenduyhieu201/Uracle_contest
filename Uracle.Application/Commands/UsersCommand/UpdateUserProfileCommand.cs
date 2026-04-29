using Uracle.Application.DTOs.StravasDto;
using Uracle.Application.DTOs.UsersDto;

namespace Uracle.Application.Commands.UsersCommand
{
    public record UpdateUserProfileCommand(string Token, string? DisplayName, string? Email, string? Bio) : ICommand<Result<UserProfileDto>>;

    public class UpdateUserProfileCommandHandler : ICommandHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<UserProfileDto>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var userId = userIdResult.Value!;
            var success = await _userRepository.UpdateProfileAsync(userId, request.DisplayName, request.Email, request.Bio, cancellationToken);
            if (!success)
                return Result<UserProfileDto>.Fail("User not found", ErrorCode.NotFound);

            var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
            return Result<UserProfileDto>.Success(new UserProfileDto
            {
                Id = user!.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Bio = user.Bio,
                StravaId = user.StravaId,
                StravaProfile = user.StravaProfile == null ? null
                    : new StravaProfileDto(user.StravaProfile.Firstname, user.StravaProfile.Lastname),
                MustChangePassword = user.MustChangePassword,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }
    }
}
