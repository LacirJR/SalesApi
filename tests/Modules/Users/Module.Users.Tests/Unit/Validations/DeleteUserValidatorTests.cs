using FluentValidation.TestHelper;
using Module.Users.Application.Commands.CreateUserCommand;
using Module.Users.Application.Commands.DeleteUserCommand;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Services;
using Module.Users.Application.Validations;
using Module.Users.Tests.Unit.Fakes;
using Xunit;

namespace Module.Users.Tests.Unit.Validations;

public class DeleteUserValidatorTests
{
    private readonly DeleteUserValidator _validator;

    public DeleteUserValidatorTests()
    {
        _validator = new DeleteUserValidator();
    }
    
    [Fact]
    public async Task Should_Have_Error_When_Guid_Is_Empty()
    {
        var command = new DeleteUserCommand(Guid.Empty);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id).WithErrorMessage("Id cannot be empty");
    }
}

