using Bogus;
using Module.Sales.Application.Commands.CancelSaleCommand;
using Module.Sales.Application.Validations;
using Xunit;

namespace Module.Sales.Tests.Unit.Validations;

public class CancelSaleValidatorTests
{
    private readonly CancelSaleValidator _validator = new();
    private readonly Faker _faker = new();

    [Fact]
    public void Should_Pass_When_SaleId_Is_Valid()
    {
        var command = new CancelSaleCommand(_faker.Random.Guid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_SaleId_Is_Empty()
    {
        var command = new CancelSaleCommand(Guid.Empty);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CancelSaleCommand.SaleId));
    }
}
