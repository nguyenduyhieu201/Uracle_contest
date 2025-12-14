using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        public Task RevokeValidTokensForUserAsync(string userId, CancellationToken cancellationToken);
    }
}
