using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.Abstractions.Security
{
    public interface IEmailService
    {
        public Task SendPasswordResetEmailAsync(string email, string firstName, string resetLink);

    }
}
