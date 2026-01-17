using FluentAssertions;
using NUnit.Framework;
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.Commands.ContestsCommand;
using Uracle.Application.Commands.TeamsCommand;
using Uracle.Application.DTOs;
using Uracle.Application.DTOs.UsersDto;

namespace Uracle.Tests.Commands;

[TestFixture]
public class ValidationTests
{
    [Test]
    public void UserLoginCommandValidator_ValidData_PassesValidation()
    {
        // Arrange
        var validator = new UserLoginCommandValidator();
        var command = new UserLoginCommand(new UserLoginDTO 
        { 
            Username = "testuser", 
            Password = "password123" 
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void UserLoginCommandValidator_EmptyUsername_FailsValidation()
    {
        // Arrange
        var validator = new UserLoginCommandValidator();
        var command = new UserLoginCommand(new UserLoginDTO 
        { 
            Username = "", 
            Password = "password123" 
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Username"));
    }

    [Test]
    public void UserLoginCommandValidator_ShortPassword_FailsValidation()
    {
        // Arrange
        var validator = new UserLoginCommandValidator();
        var command = new UserLoginCommand(new UserLoginDTO 
        { 
            Username = "testuser", 
            Password = "123" // Less than 6 characters
        });

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Password"));
    }

    [Test]
    public void ContestCreateCommandValidator_ValidData_PassesValidation()
    {
        // Arrange
        var validator = new ContestCreateCommandValidator();
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

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void ContestCreateCommandValidator_EndDateBeforeStart_FailsValidation()
    {
        // Arrange
        var validator = new ContestCreateCommandValidator();
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

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("End date"));
    }

    [Test]
    public void ContestCreateCommandValidator_MinPaceGreaterThanMax_FailsValidation()
    {
        // Arrange
        var validator = new ContestCreateCommandValidator();
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

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("pace"));
    }

    [Test]
    public void CreateTeamDTOValidator_EmptyName_FailsValidation()
    {
        // Arrange
        var validator = new CreateTeamDTOValidator();
        var dto = new CreateTeamDTO 
        { 
            Name = "", 
            Description = "Test team description" 
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Name"));
    }

    [Test]
    public void CreateTeamDTOValidator_NameTooLong_FailsValidation()
    {
        // Arrange
        var validator = new CreateTeamDTOValidator();
        var dto = new CreateTeamDTO 
        { 
            Name = new string('A', 101), // Over 100 characters
            Description = "Test team description" 
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Name"));
    }
}
