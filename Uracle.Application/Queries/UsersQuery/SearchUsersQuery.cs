using Uracle.Application.DTOs.StravasDto;
using Uracle.Application.DTOs.UsersDto;

namespace Uracle.Application.Queries.UsersQuery
{
    public record SearchUsersQuery(string Query) : IQuery<Result<List<PublicUserDto>>>;

    public class SearchUsersQueryHandler : IQueryHandler<SearchUsersQuery, Result<List<PublicUserDto>>>
    {
        private readonly IUserRepository _userRepository;

        public SearchUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<List<PublicUserDto>>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
                return Result<List<PublicUserDto>>.Fail("Query cannot be empty", ErrorCode.BadRequest);

            var users = await _userRepository.SearchByNameAsync(request.Query.Trim(), cancellationToken);
            var dtos = users.Select(u => new PublicUserDto
            {
                Id = u.Id,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Email = u.Email,
                Bio = u.Bio,
                StravaProfile = u.StravaProfile == null ? null
                    : new StravaProfileDto(u.StravaProfile.Firstname, u.StravaProfile.Lastname),
                CreatedAt = u.CreatedAt
            }).ToList();

            return Result<List<PublicUserDto>>.Success(dtos);
        }
    }
}
