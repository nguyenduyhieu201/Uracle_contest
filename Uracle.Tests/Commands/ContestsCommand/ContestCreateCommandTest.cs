using FluentAssertions;
using Moq;
using SharedKernel.Domains;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Application.Commands.ContestsCommand;
using Uracle.Application.DTOs;
using Uracle.Domain.Models;
using Uracle.Domain.Models.Contests;

namespace Uracle.Tests.Commands.ContestsCommand;

[TestFixture]
public class ContestCreateCommandTest
{
    private Mock<IContestRepository> _contestRepo = null!;
    private Mock<IGroupRepository> _groupRepo = null!;

    [SetUp]
    public void SetUp()
    {
        _contestRepo = new Mock<IContestRepository>();
        _groupRepo = new Mock<IGroupRepository>();
    }

    private ContestCreateCommandHandler CreateSut()
        => new ContestCreateCommandHandler(_contestRepo.Object, _groupRepo.Object);

    [Test]
    public async Task Handle_Success_When_ValidData()
    {
        // Arrange
        var dto = new ContestCreateDTO
        {
            GroupId = "group-id",
            Name = "Test Contest",
            Detail = "Test contest description",
            StartAt = DateTime.UtcNow.AddDays(1),
            EndAt = DateTime.UtcNow.AddDays(30),
            ContestType = ContestType.Individual,
            ActivityType = ActivityType.Run,
            MinPace = 4.0,
            MaxPace = 8.0,
            MinDistance = 3.0
        };
        var command = new ContestCreateCommand(dto, "user-id");

        _groupRepo.Setup(x => x.GetByIdAsync("group-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group { Id = "group-id", Name = "Test Group" });

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _contestRepo.Verify(x => x.AddAsync(It.Is<Contest>(c => 
            c.Name == "Test Contest" && 
            c.GroupId == "group-id" && 
            c.CreatedById == "user-id"), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_Failure_When_GroupNotFound()
    {
        // Arrange
        var dto = new ContestCreateDTO
        {
            GroupId = "nonexistent-group",
            Name = "Test Contest",
            Detail = "Test contest description",
            StartAt = DateTime.UtcNow.AddDays(1),
            EndAt = DateTime.UtcNow.AddDays(30),
            ContestType = ContestType.Individual,
            ActivityType = ActivityType.Run,
            MinPace = 4.0,
            MaxPace = 8.0,
            MinDistance = 3.0
        };
        var command = new ContestCreateCommand(dto, "user-id");

        _groupRepo.Setup(x => x.GetByIdAsync("nonexistent-group", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Contain("Group not found");
        result.ErrorCode.Should().Be(ErrorCode.NotFound);
    }

    [Test]
    public async Task Handle_Failure_When_EndDateBeforeStartDate()
    {
        // Arrange
        var dto = new ContestCreateDTO
        {
            GroupId = "group-id",
            Name = "Test Contest",
            Detail = "Test contest description",
            StartAt = DateTime.UtcNow.AddDays(10),
            EndAt = DateTime.UtcNow.AddDays(5), // End before start
            ContestType = ContestType.Individual,
            ActivityType = ActivityType.Run,
            MinPace = 4.0,
            MaxPace = 8.0,
            MinDistance = 3.0
        };
        var command = new ContestCreateCommand(dto, "user-id");

        _groupRepo.Setup(x => x.GetByIdAsync("group-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group { Id = "group-id", Name = "Test Group" });

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Contain("End date must be after start date");
        result.ErrorCode.Should().Be(ErrorCode.BadRequest);
    }

    [Test]
    public async Task Handle_Failure_When_MinPaceGreaterThanMaxPace()
    {
        // Arrange
        var dto = new ContestCreateDTO
        {
            GroupId = "group-id",
            Name = "Test Contest",
            Detail = "Test contest description",
            StartAt = DateTime.UtcNow.AddDays(1),
            EndAt = DateTime.UtcNow.AddDays(30),
            ContestType = ContestType.Individual,
            ActivityType = ActivityType.Run,
            MinPace = 8.0, // Higher than max
            MaxPace = 4.0,
            MinDistance = 3.0
        };
        var command = new ContestCreateCommand(dto, "user-id");

        _groupRepo.Setup(x => x.GetByIdAsync("group-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Group { Id = "group-id", Name = "Test Group" });

        var sut = CreateSut();

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFail.Should().BeTrue();
        result.Message.Should().Contain("Minimum pace must be less than maximum pace");
        result.ErrorCode.Should().Be(ErrorCode.BadRequest);
    }
}
