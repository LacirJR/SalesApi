using Bogus;
using Module.Sales.Application.Queries.GetSaleByIdQuery;
using Module.Sales.Application.Validations;
using Xunit;

namespace Module.Sales.Tests.Unit.Validations;

public class GetSaleByIdValidatorTests
{
    private readonly GetSaleByIdValidator _validator;
    private readonly Faker _faker;

    public GetSaleByIdValidatorTests()
    {
        _validator = new GetSaleByIdValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Should_Pass_When_SaleId_Is_Valid()
    {
        var query = new GetSaleByIdQuery(_faker.Random.Guid());

        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void Should_Fail_When_SaleId_Is_Empty(string guid)
    {
        var query = new GetSaleByIdQuery(Guid.Parse(guid));

        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.SaleId));
    }
}
