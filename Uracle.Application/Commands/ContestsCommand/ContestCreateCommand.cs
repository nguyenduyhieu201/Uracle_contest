using Uracle.Application.DTOs.ContestDto;
using Uracle.Domain.Models.Contests;

namespace Uracle.Application.Commands.ContestsCommand
{
    public record ContestCreateCommand (string token, ContestCreateRequest createContestDto) : ICommand<Result<ContestCreateResponseDto>>;
    public class ContestCreateCommandValidator : AbstractValidator<ContestCreateCommand>
    {
        public ContestCreateCommandValidator()
        {
            // Bắt buộc có CreateContestDto
            RuleFor(x => x.createContestDto)
                .NotNull()
                .WithMessage("Contest data is required.");
            // 1) Name bắt buộc phải có
            RuleFor(x => x.createContestDto.Name)
                .NotEmpty()
                .WithMessage("Contest name is required.")
                .MaximumLength(200)
                .WithMessage("Contest name must not exceed 200 characters.");
            // 2) EndAt phải lớn hơn StartAt ít nhất 1 ngày
            RuleFor(x => x.createContestDto)
                .Must(dto => dto.EndAt >= dto.StartAt.AddDays(1))
                .WithMessage("End time must be at least 1 day after start time.");
            // Thêm một vài rule cơ bản khác (tuỳ chọn)
            RuleFor(x => x.createContestDto.StartAt)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Start time must be in the future.");
        }
    }
    public class ContestCreateCommandHandler : ICommandHandler<ContestCreateCommand, Result<ContestCreateResponseDto>>
    {
        private readonly IJwtService _jWTService;
        private readonly IContestRepository _contestRepository;
        private readonly IGroupRepository _groupRepository;
        public ContestCreateCommandHandler(IJwtService jWTService, IGroupRepository groupRepository, IContestRepository contestRepository)
        {
            _jWTService = jWTService;
            _groupRepository = groupRepository;
            _contestRepository = contestRepository;
        }
        public async Task<Result<ContestCreateResponseDto>> Handle(ContestCreateCommand request, CancellationToken cancellationToken)
        {
            var userId = await _jWTService.ValidateUserAsync(request.token);
            if (userId.IsFail) 
            {
                return Result<ContestCreateResponseDto>.Fail("Invalid token",ErrorCode.Unauthorized);
            }
            var canCreate = await _groupRepository.CanManageContestAsync(request.createContestDto.GroupId, userId.Value, cancellationToken);

            if (!canCreate)
            {
                return Result<ContestCreateResponseDto>.Fail("The user is not admin",ErrorCode.Forbidden);
            }

            var contest = Contest.Create(
                        request.createContestDto.GroupId,
                        userId.Value,
                        request.createContestDto.Name,
                        request.createContestDto.Detail,
                        request.createContestDto.StartAt.UtcDateTime,
                        request.createContestDto.EndAt.UtcDateTime,
                        request.createContestDto.ContestType,
                        request.createContestDto.ActivityType,
                        request.createContestDto.MinPace,
                        request.createContestDto.MaxPace,
                        request.createContestDto.MinDistance
                    );
            await _contestRepository.AddContest(contest);   
            return Result<ContestCreateResponseDto>.Success(new ContestCreateResponseDto
            {
                ContestId = contest.Id,
                GroupId = contest.GroupId,
                CreatedById = contest.CreatedById,
                Name = contest.Name,
                Detail = contest.Detail,
                StartAt = contest.StartAt,
                EndAt = contest.EndAt,
                ContestType = contest.ContestType,
                ActivityType = contest.ActivityType,
                MinPace = contest.MinPace,
                MaxPace = contest.MaxPace,
                MinDistance = contest.MinDistance
            });
        }
    }
}
