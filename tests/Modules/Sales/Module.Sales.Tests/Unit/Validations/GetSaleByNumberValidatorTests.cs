using Bogus;
using Module.Sales.Application.Queries.GetSaleByNumberQuery;
using Module.Sales.Application.Validations;
using Xunit;

namespace Module.Sales.Tests.Unit.Validations;

public class GetSaleByNumberValidatorTests
{
    private readonly GetSaleByNumberValidator _validator;
    private readonly Faker _faker;

    public GetSaleByNumberValidatorTests()
    {
        _validator = new GetSaleByNumberValidator();
        _faker = new Faker();
    }

    [Fact]
    public void Should_Pass_When_Number_Is_Valid()
    {
        var query = new GetSaleByNumberQuery(_faker.Random.Long(1, long.MaxValue));

        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Fail_When_Number_Is_Zero_Or_Less(long number)
    {
        var query = new GetSaleByNumberQuery(number);

        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(query.Number));
    }
}
