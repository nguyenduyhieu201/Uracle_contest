using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IGroupRepository
    {
        public Task<bool> CanCreateContestAsync(string groupId, string userId, CancellationToken cancellationToken = default);

    }
}
