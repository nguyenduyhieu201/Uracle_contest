
using Uracle.Domain.Enums;

namespace Uracle.Application.Queries.ContestsQuery
{
    public record GetContestParticipantsQuery(string ContestId)
    : IQuery<Result<GetContestParticipantsResponse>>;

    public sealed class GetContestParticipantsResponse
    {
        public string ContestId { get; init; }
        public List<ContestParticipantDto> Participants { get; init; } = new List<ContestParticipantDto>();
    }

    public sealed class GetContestParticipantsQueryHandler
    : IQueryHandler<GetContestParticipantsQuery, Result<GetContestParticipantsResponse>>
    {
        private readonly IContestRepository _contestRepository;
        public GetContestParticipantsQueryHandler(IContestRepository contestRepository) => _contestRepository = contestRepository;

        public class GetContestParticipantsQueryValidator : AbstractValidator<GetContestParticipantsQuery>
        {
            public GetContestParticipantsQueryValidator()
            {
                // state phải có (còn việc state có tồn tại trong DB sẽ check ở handler)
                RuleFor(x => x.ContestId)
                    .NotEmpty()
                    .WithMessage("ContestId cannot be null");


            }
        }
        public async Task<Result<GetContestParticipantsResponse>> Handle(
            GetContestParticipantsQuery request,
            CancellationToken ct)
        {
            var contest = await _contestRepository.GetContestByIdAsync(request.ContestId, ct);
            if (contest == null)
            {
                return Result<GetContestParticipantsResponse>.Fail("Contest not found.", ErrorCode.NotFound);
            }
            if (contest.ContestType != ContestType.Individual)
            {
                return Result<GetContestParticipantsResponse>.Fail("Contest is not individual type.", ErrorCode.BadRequest);
            }
            var participants = await _contestRepository.GetParticipantsAsync(request.ContestId, ct);
            var participantDtos = participants.Select(p => new ContestParticipantDto
            {
                UserId = p.Id,
                DisplayName = !string.IsNullOrWhiteSpace(p.StravaProfile?.Firstname)
                                       && !string.IsNullOrWhiteSpace(p.StravaProfile?.Lastname)
                                ? $"{p.StravaProfile!.Firstname} {p.StravaProfile!.Lastname}"
                                : p.Username,

            }).ToList();


            var response = new GetContestParticipantsResponse
            {
                ContestId = request.ContestId,
                Participants = participantDtos
            };

            return Result<GetContestParticipantsResponse>.Success(response);
        }
    }
}
