using FluentAssertions;
using Moq;
using SharedKernel.Domains;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Application.Abstractions.Security;
using Uracle.Application.Abstractions.Services;
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs;
using Uracle.Domain.Models;

namespace Uracle.Tests.Commands.UsersCommands;

[TestFixture]
public class UserForgetPasswordCommandTest
{
    private Mock<IUserRepository> _userRepo = null!;
    private Mock<IEmailService> _emailService = null!;
    private Mock<IJwtService> _jwtService = null!;
    private Mock<IPasswordResetTokenRepository> _passwordResetTokenRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _userRepo = new Mock<IUserRepository>();
        _emailService = new Mock<IEmailService>();
        _jwtService = new Mock<IJwtService>();
        _passwordResetTokenRepository = new Mock<IPasswordResetTokenRepository>();
    }

    private UserForgetPasswordCommandHandler CreateSut()
        => new UserForgetPasswordCommandHandler(_userRepo.Object, _passwordResetTokenRepository.Object, _jwtService.Object, _emailService.Object);

    //[Test]
    //public async Task Handle_Success_When_EmailExists()
    //{
    //    // Arrange
    //    var user = SampleUser();
    //    var command = new UserForgetPasswordCommand("test@example.com");

    //    _userRepo.Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(user);

    //    var sut = CreateSut();

    //    // Act
    //    var result = await sut.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.IsSuccess.Should().BeTrue();
    //    _emailService.Verify(x => x.SendPasswordResetEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    //}

    //[Test]
    //public async Task Handle_Failure_When_EmailDoesNotExist()
    //{
    //    // Arrange
    //    var command = new UserForgetPasswordCommand("nonexistent@example.com");

    //    _userRepo.Setup(x => x.GetByEmailAsync("nonexistent@example.com", It.IsAny<CancellationToken>()))
    //        .ReturnsAsync((User?)null);

    //    var sut = CreateSut();

    //    // Act
    //    var result = await sut.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.IsSuccess.Should().BeTrue(); // Security: Don't reveal if email exists
    //    _emailService.Verify(x => x.SendPasswordResetEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    //}

    //[Test]
    //public async Task Handle_Failure_When_EmailServiceFails()
    //{
    //    // Arrange
    //    var user = SampleUser();
    //    var command = new UserForgetPasswordCommand("test@example.com");

    //    _userRepo.Setup(x => x.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(user);
    //    _emailService.Setup(x => x.SendPasswordResetEmail(user.Email, It.IsAny<string>(), It.IsAny<CancellationToken>()))
    //        .ThrowsAsync(new Exception("Email service failed"));

    //    var sut = CreateSut();

    //    // Act
    //    var result = await sut.Handle(command, CancellationToken.None);

    //    // Assert
    //    result.IsFail.Should().BeTrue();
    //    result.Message.Should().Contain("Failed to send password reset email");
    //}

    private static User SampleUser()
        => new User
        {
            Id = "user-id",
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = "hashed",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
}
