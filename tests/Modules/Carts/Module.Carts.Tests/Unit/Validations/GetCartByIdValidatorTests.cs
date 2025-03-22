using Bogus;
using FluentValidation.TestHelper;
using Module.Carts.Application.Queries.GetCartByIdQuery;
using Module.Carts.Application.Validations;
using Xunit;

namespace Module.Carts.Tests.Unit.Validations;

public class GetCartByIdValidatorTests
{
    private readonly Faker _faker;
    private readonly GetCartByIdValidator _validator;

    public GetCartByIdValidatorTests()
    {
        _faker = new Faker();
        _validator = new GetCartByIdValidator();
    }

    [Fact]
    public async Task Should_Fail_When_CartId_Is_Empty()
    {
        var query = new GetCartByIdQuery(Guid.Empty);
        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.CartId);
    }

    [Fact]
    public async Task Should_Pass_When_CartId_Is_Valid()
    {
        var query = new GetCartByIdQuery(_faker.Random.Guid());
        var result = await _validator.ValidateAsync(query);

        Assert.True(result.IsValid);
    }
}
