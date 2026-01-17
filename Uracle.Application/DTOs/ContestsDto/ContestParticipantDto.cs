namespace Uracle.Application.DTOs.ContestsDto
{
    public class ContestParticipantDto
    {
        public string UserId { get; init; }
        public string DisplayName { get; init; } = default!;
        public string FullName { get; init; } = default!;   
    }
}
