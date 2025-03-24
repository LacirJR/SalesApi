using Bogus;
using FluentValidation.TestHelper;
using Module.Carts.Application.Commands.DeleteCartCommand;
using Module.Carts.Application.Validations;
using Xunit;

namespace Module.Carts.Tests.Unit.Validations;

public class DeleteCartValidatorTests
{
    private readonly Faker _faker;
    private readonly DeleteCartValidator _validator;

    public DeleteCartValidatorTests()
    {
        _faker = new Faker();
        _validator = new DeleteCartValidator();
    }

    [Fact]
    public async Task Should_Fail_When_CartId_Is_Empty()
    {
        var command = new DeleteCartCommand(Guid.Empty);
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CartId);
    }

    [Fact]
    public async Task Should_Pass_When_CartId_Is_Valid()
    {
        var command = new DeleteCartCommand(_faker.Random.Guid());
        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }
}
