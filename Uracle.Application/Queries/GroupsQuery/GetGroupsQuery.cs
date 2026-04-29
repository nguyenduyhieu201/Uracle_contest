using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.DTOs.GroupsDto;

namespace Uracle.Application.Queries.GroupsQuery
{
    public record GetGroupsQuery(string GroupId) : IQuery<Result<List<GroupDto>>>;
    public class GetGroupsQueryHandler : IQueryHandler<GetGroupsQuery, Result<List<GroupDto>>>
    {
        private readonly IGroupRepository _groupRepository;
        public GetGroupsQueryHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }
        public async Task<Result<List<GroupDto>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            var groups = await _groupRepository
                .GetByGroupIdAsync(request.GroupId, cancellationToken);
   
            var dtos = groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.DisplayName,
                CreatedAt = g.CreatedAt
            }).ToList();
            return Result<List<GroupDto>>.Success(dtos);
        }
    }
}
