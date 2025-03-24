using Bogus;
using Module.Sales.Application.Commands.RemoveSaleItemCommand;
using Module.Sales.Application.Validations;
using Xunit;

namespace Module.Sales.Tests.Unit.Validations;

public class RemoveSaleItemValidatorTests
{
    private readonly RemoveSaleItemValidator _validator;
    private readonly Faker _faker;

    public RemoveSaleItemValidatorTests()
    {
        _validator = new RemoveSaleItemValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new RemoveSaleItemCommand(_faker.Random.Guid(), _faker.Random.Guid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_SaleId_Is_Empty()
    {
        var command = new RemoveSaleItemCommand(Guid.Empty, _faker.Random.Guid());

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SaleId));
    }

    [Fact]
    public void Should_Fail_When_ProductId_Is_Empty()
    {
        var command = new RemoveSaleItemCommand(_faker.Random.Guid(), Guid.Empty);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ProductId));
    }
}
