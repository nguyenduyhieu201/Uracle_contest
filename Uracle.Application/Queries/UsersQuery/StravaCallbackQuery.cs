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
        private IStravaService _stravaService;
        public StravaCallbackQueryHandler(IJWTService jwtService, IUserRepository userRepository, IStravaService stravaService)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _stravaService = stravaService;
        }

        public class StravaCallbackQueryValidator : AbstractValidator<StravaCallbackQuery>
        {
            public StravaCallbackQueryValidator()
            {
                // state phải có (còn việc state có tồn tại trong DB sẽ check ở handler)
                RuleFor(x => x.state)
                    .NotEmpty()
                    .WithMessage("?error=invalid_state");

                // Nếu client gửi error từ Strava về, bạn đang trả luôn ?error=missing_code
                RuleFor(x => x.error)
                    .Must(string.IsNullOrEmpty)
                    .WithMessage("?error=missing_code");

                // code bắt buộc phải có
                RuleFor(x => x.code)
                    .NotEmpty()
                    .WithMessage("?error=missing_code");
            }
        }

        public async Task<Result<StravaCallbackResponse>> Handle(StravaCallbackQuery request, CancellationToken cancellationToken)
        {
            var state = await _userRepository.FindByIdAsync(request.state, cancellationToken);
            if(state == null) return Result<StravaCallbackResponse>.Fail("?error=invalid_state");
            if (!string.IsNullOrEmpty(request.error))
            {
                return Result<StravaCallbackResponse>.Fail("?error=missing_code");
            }
            if (string.IsNullOrEmpty(request.code))
            {
                return Result<StravaCallbackResponse>.Fail("?error=missing_code");
            }

            var user = await _stravaService.HandleStravaCallback(request.code, request.error, request.state, cancellationToken);
            if (user.IsFail) return Result<StravaCallbackResponse>.Fail("?error=strava_auth_failed");
            var response = new StravaCallbackResponse("?success=true");

            return Result<StravaCallbackResponse>.Success(response);
        }
    }
}
