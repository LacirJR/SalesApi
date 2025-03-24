using Bogus;
using Module.Sales.Application.Commands.FinishSaleCommand;
using Module.Sales.Application.Validations;
using Xunit;

namespace Module.Sales.Tests.Unit.Validations;

public class FinishSaleValidatorTests
{
    private readonly FinishSaleValidator _validator;
    private readonly Faker _faker;

    public FinishSaleValidatorTests()
    {
        _validator = new FinishSaleValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Should_Pass_When_SaleId_Is_Valid()
    {
        var command = new FinishSaleCommand(_faker.Random.Guid());

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_SaleId_Is_Empty()
    {
        var command = new FinishSaleCommand(Guid.Empty);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SaleId));
    }
}
