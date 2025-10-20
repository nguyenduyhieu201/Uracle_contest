using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Domain.Models;

namespace Uracle.Application.Abstractions.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetByUserNameAsync(string username, CancellationToken cancellationToken);
    }
}
