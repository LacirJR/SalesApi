using FluentValidation.TestHelper;
using Module.Users.Application.Commands.UpdateUserCommand;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Services;
using Module.Users.Application.Validations;
using Module.Users.Tests.Unit.Fakes;
using Xunit;

namespace Module.Users.Tests.Unit.Validations;

public class UpdateUserValidatorTests
{
    private readonly IUserDomainService _userDomainService;
    private readonly UpdateUserValidator _validator;

    public UpdateUserValidatorTests()
    {
        _userDomainService = new FakeUserDomainService();
        _validator = new UpdateUserValidator(_userDomainService);
    }
    
    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new UpdateUserCommand(Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"),"invalid-email", "test", "123", new NameDto("test", "jr"),
            new AddressDto("test", "test", 1, "0000000", new GeolocationDto("12", "12")), "321123321", "Active", "Admin");

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Already_Exists()
    {
        var command = new UpdateUserCommand(Guid.Parse("e1a3bfaf-4301-47d0-8886-c56adde7ab05"),"existing@email.com", "test", "123", new NameDto("test", "jr"),
            new AddressDto("test", "test", 1, "0000000", new GeolocationDto("12", "12")), "321123321", "Active", "Admin");

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is already used.");
    }
}