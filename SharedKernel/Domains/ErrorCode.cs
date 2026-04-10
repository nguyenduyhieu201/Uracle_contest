using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domains
{
    public enum ErrorCode
    {
        None = 0,
        NotFound = 404,
        Unauthorized = 401,
        BadRequest = 400,
        Forbidden = 403,
        Conflict = 409,
        InternalError = 500
    }
}
