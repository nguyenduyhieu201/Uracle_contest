using FluentAssertions;
using Moq;
using SharedKernel.Domains;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Application.Abstractions.Security;
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs;
using Uracle.Application.DTOs.UsersDto;
using Uracle.Domain.Models;

namespace Uracle.Tests.Commands.UsersCommands
{
    public class UserRegisterCommandTest
    {
        private Mock<IUserRepository> _userRepo = null!;
        private Mock<IPasswordHasher> _hasher = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepo = new Mock<IUserRepository>();
            _hasher = new Mock<IPasswordHasher>();
        }

        private UserRegisterCommandHandler CreateSut()
            => new UserRegisterCommandHandler(_userRepo.Object, _hasher.Object);
        [Test]
        public async Task Handle_Success_When_Unique_And_StrongPassword()
        {
            var dto = new UserRegisterDTO
            {
                Email = "nguyenduyhieu202@gmail.com",
                Username = "nguyenduyhieu202",
                Password = "Str0ng!Pass"
            };
            var cmd = new UserRegisterCommand(dto);
            var cancellationToken = CancellationToken.None;

            _userRepo.Setup(r => r.GetByUserNameAsync("nguyenduyhieu202", cancellationToken))
                     .ReturnsAsync((User?)null);
            _userRepo.Setup(r => r.GetByEmailAsync("nguyenduyhieu202@gmail.com", cancellationToken))
                     .ReturnsAsync((User?)null);
            _hasher.Setup(h => h.Hash("Str0ng!Pass"))
                   .Returns("hashed");

            var sut = CreateSut();
            var result = await sut.Handle(cmd, cancellationToken);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Username.Should().Be("nguyenduyhieu202");
            result.Value.Email.Should().Be("nguyenduyhieu202@gmail.com");

            _userRepo.Verify(r => r.AddUserAsync(It.Is<User>(u =>
                u.Username == "nguyenduyhieu202" &&
                u.Email == "nguyenduyhieu202@gmail.com" &&
                u.PasswordHash == "hashed"
            ), cancellationToken), Times.Once);
        }

        [Test]
        public async Task Handle_Fail_When_Username_Exists()
        {
            var dto = new UserRegisterDTO
            {
                Email = "nguyenduyhieu202@gmail.com",
                Username = "nguyenduyhieu202",
                Password = "Str0ng!Pass"
            };
            var cmd = new UserRegisterCommand(dto);
            var cancellationToken = CancellationToken.None;

            _userRepo.Setup(r => r.GetByUserNameAsync("nguyenduyhieu202", cancellationToken))
                     .ReturnsAsync(new User { Username = "nguyenduyhieu202" });

            var sut = CreateSut();
            var result = await sut.Handle(cmd, cancellationToken);

            result.IsFail.Should().BeTrue();
            result.Message.Should().Be("Username already exists.");
            _userRepo.Verify(r => r.AddUserAsync(It.IsAny<User>(), cancellationToken), Times.Never);
        }

        [Test]
        public async Task Handle_Fail_When_Email_Exists()
        {
            var dto = new UserRegisterDTO
            {
                Email = "nguyenduyhieu202@gmail.com",
                Username = "nguyenduyhieu202",
                Password = "Str0ng!Pass"
            };
            var cmd = new UserRegisterCommand(dto);
            var cancellationToken = CancellationToken.None;

            _userRepo.Setup(r => r.GetByUserNameAsync("nguyenduyhieu202", cancellationToken))
                     .ReturnsAsync((User?)null);
            _userRepo.Setup(r => r.GetByEmailAsync("nguyenduyhieu202@gmail.com", cancellationToken))
                     .ReturnsAsync(new User { Email = "nguyenduyhieu202@gmail.com" });

            var sut = CreateSut();
            var result = await sut.Handle(cmd, cancellationToken);

            result.IsFail.Should().BeTrue();
            result.Message.Should().Be("Email already exists.");
            _userRepo.Verify(r => r.AddUserAsync(It.IsAny<User>(), cancellationToken), Times.Never);
        }

        [TestCase("short")]
        [TestCase("NoSpecial1")]
        [TestCase("nocaps1!")]
        [TestCase("NOCAPS1!")]
        public async Task Handle_Fail_When_WeakPassword(string password)
        {
            var dto = new UserRegisterDTO
            {
                Email = "nguyenduyhieu202@gmail.com",
                Username = "nguyenduyhieu202",
                Password = password
            };
            var cmd = new UserRegisterCommand(dto);
            var ct = CancellationToken.None;

            var sut = CreateSut();
            var result = await sut.Handle(cmd, ct);

            result.IsFail.Should().BeTrue();
            result.Message.Should().Contain("Password");
            _userRepo.Verify(r => r.AddUserAsync(It.IsAny<User>(), ct), Times.Never);
        }
    }
}
