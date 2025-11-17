using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Uracle.Application.Queries.UsersQuery
{
    public record StravaCallbackQuery(string code, string state, string error) : IQuery<Result<StravaCallbackResponse>>;
    public record StravaCallbackResponse(string url);
    public class StravaCallbackQueryHandler
                : IQueryHandler<StravaCallbackQuery, Result<StravaCallbackResponse>>
    {
        private IJWTService _jwtService;
        private IUserRepository _userRepository;
        private IStravaService stravaService;
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

                return Result<StravaCallbackResponse>.Fail($"?error={Uri.EscapeDataString(request.error)}");
            }
            if (string.IsNullOrEmpty(request.code))
            {
                return Result<StravaCallbackResponse>.Fail("?error=missing_code");
            }
            var loggedInUserId = token.Value;
            var user = await stravaService.HandleStravaCallback(request.code, request.error, loggedInUserId);

            //var user = 
        }
    }
}
