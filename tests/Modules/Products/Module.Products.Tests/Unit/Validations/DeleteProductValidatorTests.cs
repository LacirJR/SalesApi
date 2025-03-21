using FluentValidation.TestHelper;
using Module.Products.Application.Commands.DeleteProductCommand;
using Module.Products.Application.Validations;
using Xunit;

namespace Module.Products.Tests.Unit.Validations;

public class DeleteProductValidatorTests
{
    private readonly DeleteProductValidator _validator;

    public DeleteProductValidatorTests()
    {
        _validator = new DeleteProductValidator();
    }

    [Fact]
    public async Task Should_Validate_Successfully_When_Id_Is_Valid()
    {
        var command = new DeleteProductCommand(Guid.NewGuid());

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_Fail_When_Id_Is_Empty()
    {
        var command = new DeleteProductCommand(Guid.Empty);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required");
    }
}