using FluentValidation.TestHelper;
using Module.Users.Application.Commands.CreateUserCommand;
using Module.Users.Application.Dtos;
using Module.Users.Application.Interfaces.Services;
using Module.Users.Application.Validations;
using Module.Users.Tests.Unit.Fakes;
using Xunit;

namespace Module.Users.Tests.Unit.Validations;

public class CreateUserValidatorTests
{
    private readonly IUserDomainService _userDomainService;
    private readonly CreateUserValidator _validator;

    public CreateUserValidatorTests()
    {
        _userDomainService = new FakeUserDomainService();
        _validator = new CreateUserValidator(_userDomainService);
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new CreateUserCommand("invalid-email", "test", "123", new NameDto("test", "jr"),
            new AddressDto("test", "test", 1, "0000000", new GeolocationDto("12", "12")), "321123321", "Active", "Admin");

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Invalid email format.");
    }

    [Fact]
    public async Task Should_Have_Error_When_Email_Already_Exists()
    {
        var command = new CreateUserCommand("existing@email.com", "test", "admin123@", new NameDto("test", "jr"),
            new AddressDto("test", "test", 1, "0000000", new GeolocationDto("12", "12")), "321123321", "Active", "Admin");

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Email).WithErrorMessage("Email is already used.");
    }
}