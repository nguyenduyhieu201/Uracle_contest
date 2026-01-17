using FluentAssertions;
using Uracle.Application.Commands.UsersCommand;
using Uracle.Application.DTOs;
using Uracle.Application.DTOs.UsersDto;
namespace Uracle.Tests.Commands.UsersCommands
{
    [TestFixture]
    public class UserLoginCommandValidatorTests
    {
        [Test]
        public void Validate_ShouldFail_WhenUsernameIsEmpty()
        {
            var validator = new UserLoginCommandValidator();
            var cmd = new UserLoginCommand(new UserLoginDTO("", "abcdef"));
            var result = validator.Validate(cmd);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Validate_ShouldFail_WhenPasswordShorterThan6Chars()
        {
            var validator = new UserLoginCommandValidator();
            var cmd = new UserLoginCommand(new UserLoginDTO("john", "abc"));
            var result = validator.Validate(cmd);
            result.IsValid.Should().BeFalse();
        }

        [Test]
        public void Validate_ShouldPass_WhenUsernameAndPasswordValid()
        {
            var validator = new UserLoginCommandValidator();
            var cmd = new UserLoginCommand(new UserLoginDTO("john", "abcdef"));
            var result = validator.Validate(cmd);
            result.IsValid.Should().BeTrue();
        }
    }
}
