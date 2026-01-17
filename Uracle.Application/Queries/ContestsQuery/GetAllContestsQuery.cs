
namespace Uracle.Application.Queries.ContestsQuery
{
    public record GetAllContestsQuery(string token) : IQuery<Result<List<ContestDto>>>;

    public class GetUserContestsQueryHandler
    : IQueryHandler<GetAllContestsQuery, Result<List<ContestDto>>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IContestRepository _contestRepository;
        private readonly IJwtService _jwtService; 

        public GetUserContestsQueryHandler(
            IGroupRepository groupRepository,
            IContestRepository contestRepository,
            IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _contestRepository = contestRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<List<ContestDto>>> Handle(
            GetAllContestsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = await _jwtService.ValidateUserAsync(request.token);
            if (userId.IsFail)
            {
                return Result<List<ContestDto>>.Fail("Invalid token", ErrorCode.Unauthorized);
            }
            // 1. Lấy group memberships của user
            var memberships = await _groupRepository
                            .FindByUserId(userId.Value, cancellationToken);

            if (memberships.Count == 0)
            {
                // User không ở group nào -> trả list rỗng vẫn là Success
                return Result<List<ContestDto>>.Success(new List<ContestDto>());
            }

            var groupIds = memberships
                .Select(m => m.GroupId)
                .Distinct()
                .ToList();

            // 2. Lấy contests theo groupIds
            var contests = await _contestRepository
                .GetByGroupIdsAsync(groupIds, cancellationToken);

            // 3. Map sang DTO
            var dtos = contests.Select(c => new ContestDto
            {
                Id = c.Id,
                GroupId = c.GroupId,
                Name = c.Name,
                StartAt = c.StartAt,
                EndAt = c.EndAt,
                ContestType = c.ContestType
            }).ToList();

            return Result<List<ContestDto>>.Success(dtos);
        }
    }
}
