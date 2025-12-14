using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.DTOs.ContestDto;
using Uracle.Domain.Models.Contests;

namespace Uracle.Application.Commands.ContestsCommand
{
    public record ContestCreateCommand (string token, ContestCreateRequestDto createContestDto) : ICommand<Result<ContestCreateResponseDto>>;
    public class ContestCreateCommandValidator : AbstractValidator<ContestCreateCommand>
    {

    }
    public class ContestCreateCommandHandler : ICommandHandler<ContestCreateCommand, Result<ContestCreateResponseDto>>
    {
        public IJWTService _jWTService;
        public IContestRepository _contestRepository;
        public IGroupRepository _groupRepository;
        public ContestCreateCommandHandler(IJWTService jWTService, IGroupRepository groupRepository, IContestRepository contestRepository)
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
                return Result<ContestCreateResponseDto>.Fail("Invalid token");
            }
            var canCreate = await _groupRepository.CanCreateContestAsync(request.createContestDto.GroupId, userId.Value, cancellationToken);

            if (!canCreate)
            {
                return Result<ContestCreateResponseDto>.Fail("The user is not admin");
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
                CreatedBy = contest.CreatedBy,

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
