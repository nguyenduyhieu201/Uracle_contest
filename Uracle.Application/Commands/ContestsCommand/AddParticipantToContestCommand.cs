using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Enums;

namespace Uracle.Application.Commands.ContestsCommand
{
    public sealed record AddParticipantToContestCommand(
            string ContestId,
            string ParticipantId
        ) : ICommand<Result<ContestUser>>;
    public class AddParticipantToContestCommandHandler : ICommandHandler<AddParticipantToContestCommand, Result<ContestUser>>
    {
        private readonly IContestRepository _contestRepository;
        private readonly IUserTeamsCacheService _userTeamsCacheService;

        public AddParticipantToContestCommandHandler(IContestRepository contestRepository, IUserTeamsCacheService userTeamsCacheService)
        {
            _contestRepository = contestRepository;
            _userTeamsCacheService = userTeamsCacheService;
        }
        public async Task<Result<ContestUser>> Handle(AddParticipantToContestCommand request, CancellationToken cancellationToken)
        {
            var contest = await _contestRepository.GetContestByIdAsync(request.ContestId, cancellationToken);
            if (contest == null)
            {
                return Result<ContestUser>.Fail("Contest not found.", ErrorCode.NotFound);
            }
            if (contest.HasStarted(DateTime.UtcNow))
            {
                return Result<ContestUser>.Fail("Cannot add participant to a contest that has already started.", ErrorCode.BadRequest);
            }
            var isExists = await _contestRepository.IsUserInContestAsync(request.ContestId, request.ParticipantId, cancellationToken);
            if (isExists)
            {
                return Result<ContestUser>.Fail("Participant already in contest.", ErrorCode.BadRequest);
            }
            var isIndividual = contest.ContestType == ContestType.Individual;
            if (!isIndividual)
            {
                return Result<ContestUser>.Fail("Cannot add participant to a non-individual contest directly.", ErrorCode.BadRequest);
            }
            var contestUser = ContestUser.Create(request.ContestId, request.ParticipantId);
            if (contestUser.IsFail)
            {
                return Result<ContestUser>.Fail(contestUser.Message ?? "Failed to create contest user.", contestUser.ErrorCode);
            }
            await _contestRepository.AddContestUserAsync(contestUser.Value, cancellationToken);
            return Result<ContestUser>.Success(contestUser.Value); 
        }
    }
}
