using FluentAssertions;
using Moq;
using SharedKernel.Domains;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Application.Abstractions.Security;
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs;
using Uracle.Application.DTOs.UsersDto;
using Uracle.Domain.Models;

namespace Uracle.Tests.Commands.UsersCommands;

[TestFixture]
public class UserLoginCommandTest
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

    private UserLoginCommandHandler CreateSut()
        => new UserLoginCommandHandler(_userRepo.Object, _passwordHasher.Object, _jwtService.Object);

    [Test]
    public async Task Handle_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var user = SampleUser();
        var loginDto = new UserLoginDTO { Username = "testuser", Password = "password123" };
        var command = new UserLoginCommand(loginDto);

        _userRepo.Setup(x => x.GetByUserNameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify("password123", "hashed"))
            .Returns(true);
        _jwtService.Setup(x => x.GenerateToken(user))
            .Returns("jwt-token");
        _jwtService.Setup(x => x.GenerateRefreshToken(user))
            .Returns("refresh-token");

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Username.Should().Be("testuser");
        result.Value.JwtToken.Should().Be("jwt-token");
        result.Value.RefreshToken.Should().Be("refresh-token");
        
        _userRepo.Verify(x => x.SetRefreshTokenAsync(user.Id, "refresh-token", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_InvalidUsername_ReturnsFailure()
    {
        // Arrange
        var loginDto = new UserLoginDTO { Username = "nonexistent", Password = "password123" };
        var command = new UserLoginCommand(loginDto);

        _userRepo.Setup(x => x.GetByUserNameAsync("nonexistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Be("Invalid username or password.");
        result.ErrorCode.Should().Be(ErrorCode.BadRequest);
    }

    [Test]
    public async Task Handle_InvalidPassword_ReturnsFailure()
    {
        // Arrange
        var user = SampleUser();
        var loginDto = new UserLoginDTO { Username = "testuser", Password = "wrongpassword" };
        var command = new UserLoginCommand(loginDto);

        _userRepo.Setup(x => x.GetByUserNameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify("wrongpassword", "hashed"))
            .Returns(false);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Be("Invalid username or password.");
        result.ErrorCode.Should().Be(ErrorCode.BadRequest);
    }

    [Test]
    public async Task Handle_InactiveUser_ReturnsFailure()
    {
        // Arrange
        var user = SampleUser();
        user.IsActive = false;
        var loginDto = new UserLoginDTO { Username = "testuser", Password = "password123" };
        var command = new UserLoginCommand(loginDto);

        _userRepo.Setup(x => x.GetByUserNameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(x => x.Verify("password123", "hashed"))
            .Returns(true);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Be("Invalid username or password.");
        result.ErrorCode.Should().Be(ErrorCode.BadRequest);
    }

    private static User SampleUser()
        => new User
        {
            Id = "user-id",
            Username = "testuser",
            PasswordHash = "hashed",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
}
