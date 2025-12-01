using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.Abstractions.Security;

namespace Uracle.Infrastructure.Security
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        private IConfiguration _configuration;
        public BCryptPasswordHasher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Hash(string password)
        {
            var workFactor = int.Parse(_configuration["WorkFactor"]);
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
