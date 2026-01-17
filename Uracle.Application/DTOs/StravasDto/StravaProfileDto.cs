using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs.StravasDto
{
    public sealed record StravaProfileDto(
        string? Firstname,
        string? Lastname
    );
}
