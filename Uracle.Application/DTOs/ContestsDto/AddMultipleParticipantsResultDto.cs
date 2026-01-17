using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.ContestsDto
{
    public sealed record AddMultipleParticipantsRequest(
        List<string> ParticipantIds
    );
    public sealed record AddMultipleParticipantsFailedItemDto(
        string ParticipantId,
        string Reason
    );
    public sealed record AddMultipleParticipantsResultDto(
        List<string> Successful,
        List<AddMultipleParticipantsFailedItemDto> Failed
    );
}
