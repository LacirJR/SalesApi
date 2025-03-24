using Bogus;
using Module.Sales.Application.Commands.UpdateSaleCommand;
using Module.Sales.Application.Validations;
using Xunit;

namespace Module.Sales.Tests.Unit.Validations;

public class UpdateSaleValidatorTests
{
    private readonly UpdateSaleValidator _validator;
    private readonly Faker _faker;

    public UpdateSaleValidatorTests()
    {
        _validator = new UpdateSaleValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Should_Pass_When_Command_Is_Valid()
    {
        var command = new UpdateSaleCommand(
            _faker.Random.Guid(),
            _faker.Company.CompanyName(),
            _faker.Date.PastOffset().DateTime);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_SaleId_Is_Empty()
    {
        var command = new UpdateSaleCommand(
            Guid.Empty,
            _faker.Company.CompanyName(),
            _faker.Date.PastOffset().DateTime);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.SaleId));
    }

    [Fact]
    public void Should_Fail_When_Branch_Is_Empty()
    {
        var command = new UpdateSaleCommand(
            _faker.Random.Guid(),
            "",
            _faker.Date.PastOffset().DateTime);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Branch));
    }

    [Fact]
    public void Should_Fail_When_Branch_Exceeds_MaxLength()
    {
        var command = new UpdateSaleCommand(
            _faker.Random.Guid(),
            _faker.Random.String2(101),
            _faker.Date.PastOffset().DateTime);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Branch));
    }

    [Fact]
    public void Should_Fail_When_Date_Is_In_The_Future()
    {
        var command = new UpdateSaleCommand(
            _faker.Random.Guid(),
            _faker.Company.CompanyName(),
            _faker.Date.FutureOffset().DateTime);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Date));
    }
}
