using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.DTOs
{
    public class GroupDto
    {
        public string Id { get; set; }
        public object Name { get; set; }
        public object Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
