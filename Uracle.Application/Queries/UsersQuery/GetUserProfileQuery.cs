using Uracle.Application.DTOs.StravasDto;
using Uracle.Application.DTOs.UsersDto;

namespace Uracle.Application.Queries.UsersQuery
{
    public record GetUserProfileQuery(string Token) : IQuery<Result<UserProfileDto>>;

    public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public GetUserProfileQueryHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<UserProfileDto>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var user = await _userRepository.GetUserByIdAsync(userIdResult.Value!, cancellationToken);
            if (user == null)
                return Result<UserProfileDto>.Fail("User not found", ErrorCode.NotFound);

            return Result<UserProfileDto>.Success(new UserProfileDto
            {
                Id = user.Id,
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
