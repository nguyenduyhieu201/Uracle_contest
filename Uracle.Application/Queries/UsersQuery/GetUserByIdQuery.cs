using Uracle.Application.DTOs.StravasDto;
using Uracle.Application.DTOs.UsersDto;

namespace Uracle.Application.Queries.UsersQuery
{
    public record GetUserByIdQuery(string TargetUserId) : IQuery<Result<PublicUserDto>>;

    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Result<PublicUserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<PublicUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.TargetUserId, cancellationToken);
            if (user == null)
                return Result<PublicUserDto>.Fail("User not found", ErrorCode.NotFound);

            return Result<PublicUserDto>.Success(new PublicUserDto
            {
                Id = user.Id,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Bio = user.Bio,
                StravaProfile = user.StravaProfile == null ? null
                    : new StravaProfileDto(user.StravaProfile.Firstname, user.StravaProfile.Lastname),
                CreatedAt = user.CreatedAt
            });
        }
    }
}
