namespace Uracle.Application.Commands.UsersCommand
{
    public record RefreshTokenCommand(string RefreshToken) : ICommand<Result<RefreshTokenResponse>>;
    public record RefreshTokenResponse(string AccessToken, string NewRefreshToken, UserInfo User);
    public record UserInfo(string Id, string Username, string? Email);

    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public RefreshTokenCommandHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result<RefreshTokenResponse>.Fail("Refresh token is required", ErrorCode.BadRequest);

            // Validate the refresh token (it's a JWT with "type":"refresh" claim)
            var userIdResult = await _jwtService.ValidateUserAsync(request.RefreshToken, cancellationToken);
            if (userIdResult.IsFail)
                return Result<RefreshTokenResponse>.Fail("Invalid refresh token", ErrorCode.Unauthorized);

            var user = await _userRepository.GetUserByIdAsync(userIdResult.Value!, cancellationToken);
            if (user == null)
                return Result<RefreshTokenResponse>.Fail("User not found", ErrorCode.NotFound);

            // Verify stored refresh token matches
            if (user.JwtRefreshToken != request.RefreshToken)
                return Result<RefreshTokenResponse>.Fail("Refresh token has been revoked", ErrorCode.Unauthorized);

            // Generate new tokens
            var newAccessToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken(user);

            // Store new refresh token
            await _userRepository.SetRefreshTokenAsync(user.Id, newRefreshToken, cancellationToken);

            return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(
                newAccessToken,
                newRefreshToken,
                new UserInfo(user.Id, user.Username, user.Email)));
        }
    }
}

