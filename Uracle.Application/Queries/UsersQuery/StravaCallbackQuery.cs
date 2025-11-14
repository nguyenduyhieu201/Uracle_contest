using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uracle.Application.Queries.UsersQuery
{
    public record StravaCallbackQuery(string code, string state, string error) : IQuery<Result<StravaCallbackResponse>>;
    public record StravaCallbackResponse(string url);
    public class StravaCallbackQueryHandler
                : IQueryHandler<StravaCallbackQuery, Result<StravaCallbackResponse>>
    {
        private IJWTService _jwtService;
        private IUserRepository _userRepository;
        public StravaCallbackQueryHandler(IJWTService jwtService, IUserRepository userRepository)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
        }
        public async Task<Result<StravaCallbackResponse>> Handle(StravaCallbackQuery request, CancellationToken cancellationToken)
        {
            var token = await _jwtService.ValidateUserAsync(request.state);
            if(token.IsFail) return Result<StravaCallbackResponse>.Fail("?error=invalid_state");
            if (!string.IsNullOrEmpty(request.error))
            {
                return Result<StravaCallbackResponse>.Fail("?error=missing_code");
            }
            if (string.IsNullOrEmpty(request.code))
            {
                return Result<StravaCallbackResponse>.Fail("?error=missing_code");
            }
            var user = 
        }
    }
}
