using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Enums;

namespace Uracle.Application.DTOs.ContestDto
{
    public class ContestUpdateDto
    {
        public string? Name { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public ContestType? ContestType { get; set; }
    }
}
