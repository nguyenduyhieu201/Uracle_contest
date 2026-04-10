using FluentAssertions;
using Moq;
using SharedKernel.Domains;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Application.Abstractions.Security;
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs;
using Uracle.Application.DTOs.UsersDto;
using Uracle.Domain.Models;

namespace Uracle.Tests;

[TestFixture]
public class UsersLoginCommandTest
{
    private Mock<IUserRepository> _userRepo = null!;
    private Mock<IPasswordHasher> _passwordHasher = null!;
    private Mock<IJwtService> _jwtService = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepo = new Mock<IUserRepository>();
        _passwordHasher = new Mock<IPasswordHasher>();
        _jwtService = new Mock<IJwtService>();
    }

    private UserLoginCommandHandler CreateHandler() => new UserLoginCommandHandler(
        _userRepo.Object,
        _passwordHasher.Object,
        _jwtService.Object);

    private static User SampleUser()
            => new User
            {
                Username = "nguyenduyhieu202",
                PasswordHash = "hashed",
                IsActive = true
            };

    [Test]
    public async Task Handle_ReturnsSuccess_WhenCredentialsValid()
    {
        var user = SampleUser();
        _userRepo.Setup(x => x.GetByUserNameAsync("nguyenduyhieu202", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify("P@ssw0rd", user.PasswordHash)).Returns(true);
        _jwtService.Setup(x => x.GenerateToken(user)).Returns("jwt-token");
        _jwtService.Setup(x => x.GenerateRefreshToken(user)).Returns("refresh-token");

        var handler = CreateHandler();
        var cmd = new UserLoginCommand(new UserLoginDTO("nguyenduyhieu202", "P@ssw0rd"));

        Result<LoginResponseDto> result = await handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Username.Should().Be("nguyenduyhieu202");
        result.Value.JwtToken.Should().Be("jwt-token");
        result.Value.RefreshToken.Should().Be("refresh-token");
    }

    [Test]
    public async Task Handle_ReturnsFail_WhenUserNotFound()
    {
        _userRepo.Setup(x => x.GetByUserNameAsync("unknown", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((User?)null);

        var handler = CreateHandler();
        var cmd = new UserLoginCommand(new UserLoginDTO("unknown", "whatever"));

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Invalid username or password.");
    }

    [Test]
    public async Task Handle_ReturnsFail_WhenPasswordInvalid()
    {
        var user = SampleUser();
        _userRepo.Setup(x => x.GetByUserNameAsync("nguyenduyhieu202", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify("wrong", user.PasswordHash)).Returns(false);

        var handler = CreateHandler();
        var cmd = new UserLoginCommand(new UserLoginDTO("nguyenduyhieu202", "wrong"));

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Be("Invalid username or password.");
    }
}
