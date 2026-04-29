namespace Uracle.Application.Commands.GroupsCommand
{
    // Request to join
    public record RequestToJoinGroupCommand(string GroupId, string Token) : ICommand<Result<bool>>;

    public class RequestToJoinGroupCommandHandler : ICommandHandler<RequestToJoinGroupCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public RequestToJoinGroupCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<bool>> Handle(RequestToJoinGroupCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<bool>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var userId = userIdResult.Value!;
            var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
            if (group == null)
                return Result<bool>.Fail("Group not found", ErrorCode.NotFound);

            var isMember = await _groupRepository.IsMemberAsync(request.GroupId, userId, cancellationToken);
            if (isMember)
                return Result<bool>.Fail("User is already a member of this group", ErrorCode.BadRequest);

            var hasPending = await _groupRepository.HasPendingJoinRequestAsync(request.GroupId, userId, cancellationToken);
            if (hasPending)
                return Result<bool>.Fail("You already have a pending join request", ErrorCode.BadRequest);

            await _groupRepository.CreateJoinRequestAsync(request.GroupId, userId, cancellationToken);
            return Result<bool>.Success(true);
        }
    }

    // Approve join request
    public record ApproveJoinRequestCommand(string GroupId, string TargetUserId, string Token) : ICommand<Result<bool>>;

    public class ApproveJoinRequestCommandHandler : ICommandHandler<ApproveJoinRequestCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public ApproveJoinRequestCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<bool>> Handle(ApproveJoinRequestCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<bool>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var isAdmin = await _groupRepository.IsUserAdminInGroup(userIdResult.Value!, request.GroupId, cancellationToken);
            if (!isAdmin)
                return Result<bool>.Fail("Only admins can approve join requests", ErrorCode.Forbidden);

            var success = await _groupRepository.ApproveJoinRequestAsync(request.GroupId, request.TargetUserId, cancellationToken);
            if (!success)
                return Result<bool>.Fail("Join request not found", ErrorCode.NotFound);

            return Result<bool>.Success(true);
        }
    }

    // Reject join request
    public record RejectJoinRequestCommand(string GroupId, string TargetUserId, string Token) : ICommand<Result<bool>>;

    public class RejectJoinRequestCommandHandler : ICommandHandler<RejectJoinRequestCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public RejectJoinRequestCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<bool>> Handle(RejectJoinRequestCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<bool>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var isAdmin = await _groupRepository.IsUserAdminInGroup(userIdResult.Value!, request.GroupId, cancellationToken);
            if (!isAdmin)
                return Result<bool>.Fail("Only admins can reject join requests", ErrorCode.Forbidden);

            var success = await _groupRepository.RejectJoinRequestAsync(request.GroupId, request.TargetUserId, cancellationToken);
            if (!success)
                return Result<bool>.Fail("Join request not found", ErrorCode.NotFound);

            return Result<bool>.Success(true);
        }
    }

    // Revoke join request
    public record RevokeJoinRequestCommand(string GroupId, string Token) : ICommand<Result<bool>>;

    public class RevokeJoinRequestCommandHandler : ICommandHandler<RevokeJoinRequestCommand, Result<bool>>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IJwtService _jwtService;

        public RevokeJoinRequestCommandHandler(IGroupRepository groupRepository, IJwtService jwtService)
        {
            _groupRepository = groupRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<bool>> Handle(RevokeJoinRequestCommand request, CancellationToken cancellationToken)
        {
            var userIdResult = await _jwtService.ValidateUserAsync(request.Token, cancellationToken);
            if (userIdResult.IsFail)
                return Result<bool>.Fail(userIdResult.Message!, userIdResult.ErrorCode);

            var userId = userIdResult.Value!;
            var hasPending = await _groupRepository.HasPendingJoinRequestAsync(request.GroupId, userId, cancellationToken);
            if (!hasPending)
                return Result<bool>.Fail("No pending join request found", ErrorCode.NotFound);

            var success = await _groupRepository.RevokeJoinRequestAsync(request.GroupId, userId, cancellationToken);
            if (!success)
                return Result<bool>.Fail("Failed to revoke join request", ErrorCode.NotFound);

            return Result<bool>.Success(true);
        }
    }
}
