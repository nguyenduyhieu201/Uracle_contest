using FluentAssertions;
using Moq;
using SharedKernel.Domains;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Application.Commands.ContestsCommand;
using Uracle.Domain.Models;
using Uracle.Domain.Models.Contests;

namespace Uracle.Tests.Commands.ContestsCommand;

[TestFixture]
public class AddParticipantToContestCommandTest
{
    private Mock<IContestRepository> _contestRepo = null!;
    private Mock<IUserRepository> _userRepo = null!;
    //private Mock<IContestUserRepository> _contestUserRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _contestRepo = new Mock<IContestRepository>();
        _userRepo = new Mock<IUserRepository>();
        //_contestUserRepo = new Mock<IContestUserRepository>();
    }

    private AddParticipantToContestCommandHandler CreateSut()
        => new AddParticipantToContestCommandHandler(_contestRepo.Object, _userRepo.Object);

    [Test]
    public async Task Handle_Success_When_ValidParticipant()
    {
        // Arrange
        var contest = SampleContest();
        var user = SampleUser();
        var command = new AddParticipantToContestCommand("contest-id", "user-id");

        _contestRepo.Setup(x => x.GetContestByIdAsync("contest-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contest);
        _userRepo.Setup(x => x.GetUserByIdAsync("user-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _contestRepo.Setup(x => x.IsUserInContestAsync("user-id", "contest-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _contestRepo.Verify(x => x.AddContestUserAsync(It.Is<ContestUser>(cu => 
            cu.ContestId == "contest-id" && 
            cu.UserId == "user-id"), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_Failure_When_ContestNotFound()
    {
        // Arrange
        var command = new AddParticipantToContestCommand("nonexistent-contest", "user-id");

        _contestRepo.Setup(x => x.GetContestByIdAsync("nonexistent-contest", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contest?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Contain("Contest not found");
        result.ErrorCode.Should().Be(ErrorCode.NotFound);
    }

    [Test]
    public async Task Handle_Failure_When_UserNotFound()
    {
        // Arrange
        var contest = SampleContest();
        var command = new AddParticipantToContestCommand("contest-id", "nonexistent-user");

        _contestRepo.Setup(x => x.GetContestByIdAsync("contest-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contest);
        _userRepo.Setup(x => x.GetUserByIdAsync("nonexistent-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Contain("User not found");
        result.ErrorCode.Should().Be(ErrorCode.NotFound);
    }

    [Test]
    public async Task Handle_Failure_When_UserAlreadyInContest()
    {
        // Arrange
        var contest = SampleContest();
        var user = SampleUser();
        var command = new AddParticipantToContestCommand("contest-id", "user-id");

        _contestRepo.Setup(x => x.GetContestByIdAsync("contest-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(contest);
        _userRepo.Setup(x => x.GetUserByIdAsync("user-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        //_contestUserRepo.Setup(x => x.IsUserInContestAsync("user-id", "contest-id", It.IsAny<CancellationToken>()))
        //    .ReturnsAsync(true);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Contain("User is already in this contest");
        result.ErrorCode.Should().Be(ErrorCode.BadRequest);
    }

    private static Contest SampleContest()
        => new Contest
        {
            Id = "contest-id",
            Name = "Test Contest",
            GroupId = "group-id",
            StartAt = DateTime.UtcNow.AddDays(1),
            EndAt = DateTime.UtcNow.AddDays(30),
            ContestType = ContestType.Individual,
            ActivityType = ActivityType.Run
        };

    private static User SampleUser()
        => new User
        {
            Id = "user-id",
            Username = "testuser",
            Email = "test@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
}
