using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.Abstractions.Security
{
    public interface IStravaService
    {
        public Task<Result<string>> HandleAuthorizeUrl(string token);
        public Task<Result<User?>> HandleStravaCallback(string code, string? error, string userId, CancellationToken cancellationToken);
    }
}
