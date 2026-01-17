namespace Uracle.Application.DTOs.ContestsDto
{
    public sealed record AvailableContestParticipantDto(
        string UserId,
        string Name,
        string Username,
        StravaProfileDto? StravaProfile
    );

}
