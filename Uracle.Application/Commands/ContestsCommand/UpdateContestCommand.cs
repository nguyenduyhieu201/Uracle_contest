using Uracle.Application.DTOs.ContestDto;
using Uracle.Domain.Models.Contests;

namespace Uracle.Application.Commands.ContestsCommand
{
    public record UpdateContestCommand(
                        string ContestId,
                        string token,
                        ContestUpdateDto contestUpdateDto
                    ) : ICommand<Result<ContestDto>>;
    public class UpdateContestCommandValidator : AbstractValidator<UpdateContestCommand>
    {
        public UpdateContestCommandValidator()
        {
            RuleFor(x => x.contestUpdateDto)
                        .NotNull()
                        .WithMessage("Contest data is required.");
            RuleFor(x => x.contestUpdateDto)
                .Must(dto =>
                    !dto.StartAt.HasValue ||
                    !dto.EndAt.HasValue ||
                    // Nếu cả hai đều có thì mới check chênh lệch 1 ngày
                    dto.EndAt.Value >= dto.StartAt.Value.AddDays(1)
                )
                .WithMessage("End time must be at least 1 day after start time when both start and end times are provided.");
        }
    }
    public class UpdateContestCommandHandler : ICommandHandler<UpdateContestCommand, Result<ContestDto>>
    {
        private IContestRepository _contestRepository;
        private IGroupRepository _groupRepository;
        private IJwtService _jwtService;
        public UpdateContestCommandHandler(IContestRepository contestRepository,
            IGroupRepository groupRepository, IJwtService jwtService
            )
        {
            _contestRepository = contestRepository;
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }
        public async Task<Result<ContestDto>> Handle(UpdateContestCommand request, CancellationToken cancellationToken)
        {
            var userId = await _jwtService.ValidateUserAsync(request.token);
            if(userId.IsFail) return Result<ContestDto>.Fail("Unauthorized", ErrorCode.Unauthorized);
            var groupId = await _contestRepository.GetGroupByContestAsync(request.ContestId, cancellationToken);
            if (string.IsNullOrEmpty(groupId))
                return Result<ContestDto>.Fail("Group not found ", ErrorCode.NotFound);
            var canManage = await _groupRepository
                 .IsUserAdminInGroup(userId.Value, groupId, cancellationToken);

            if (!canManage)
                return Result<ContestDto>.Fail("Forbidden", ErrorCode.Forbidden);

            var contest = await _contestRepository
                .GetContestByIdAsync(request.ContestId, cancellationToken);
            
            if (contest is null)
                return Result<ContestDto>.Fail("ContestNotFound", ErrorCode.NotFound);
            var isDateAvaible = IsDateAvaible(contest, request.contestUpdateDto);
            if (!isDateAvaible)
            {
                return Result<ContestDto>.Fail("Invalid date range",ErrorCode.BadRequest);
            }
            var p = request.contestUpdateDto;

            // Domain method Update hoặc set trực tiếp
            contest.Update(
                p.Name,
                p.StartAt,
                p.EndAt,
                p.ContestType
            );

            await _contestRepository.UpdateAsync(contest, cancellationToken);

            var dto = new ContestDto
            {
                Id = contest.Id,
                GroupId = contest.GroupId,
                Name = contest.Name,
                StartAt = contest.StartAt,
                EndAt = contest.EndAt,
                ContestType = contest.ContestType
            };

            return Result<ContestDto>.Success(dto);
        }
    
        private bool IsDateAvaible(Contest contest, ContestUpdateDto contestUpdateDto)
        {
            if (contestUpdateDto.StartAt.HasValue)
            {
                return contestUpdateDto.EndAt >= contestUpdateDto.StartAt.Value.AddDays(1);
            }
            else if (contestUpdateDto.EndAt.HasValue)
            {
                return contestUpdateDto.EndAt.Value >= contest.StartAt.AddDays(1);
            }
            return true;
        }
    }
}
