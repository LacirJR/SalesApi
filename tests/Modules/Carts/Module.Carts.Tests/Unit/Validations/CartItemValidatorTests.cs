using Bogus;
using FluentValidation.TestHelper;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Validations;
using NSubstitute;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Services;
using Xunit;

namespace Module.Carts.Tests.Unit.Validations;

public class CartItemValidatorTests
{
    private readonly Faker _faker;
    private readonly ISharedProductService _sharedProductService;
    private readonly CartItemValidator _validator;

    public CartItemValidatorTests()
    {
        _faker = new Faker();
        _sharedProductService = Substitute.For<ISharedProductService>();

        _validator = new CartItemValidator(_sharedProductService);
    }

    [Fact]
    public async Task Should_Fail_When_ProductId_Is_Empty()
    {
        var item = new CartItemDto { ProductId = Guid.Empty, Quantity = _faker.Random.Int(1, 20) };
        var result = await _validator.TestValidateAsync(item);

        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public async Task Should_Fail_When_Product_Does_Not_Exist()
    {
        var productId = _faker.Random.Guid();
        _sharedProductService.GetProductByIdAsync(productId, Arg.Any<CancellationToken>()).Returns((SharedProductDto?)null);

        var item = new CartItemDto { ProductId = productId, Quantity = _faker.Random.Int(1, 20) };
        var result = await _validator.TestValidateAsync(item);

        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public async Task Should_Fail_When_Quantity_Is_Less_Than_One()
    {
        var item = new CartItemDto { ProductId = _faker.Random.Guid(), Quantity = 0 };
        var result = await _validator.TestValidateAsync(item);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public async Task Should_Fail_When_Quantity_Exceeds_Limit()
    {
        var item = new CartItemDto { ProductId = _faker.Random.Guid(), Quantity = 21 };
        var result = await _validator.TestValidateAsync(item);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var productId = _faker.Random.Guid();
        _sharedProductService.GetProductByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(new SharedProductDto());

        var item = new CartItemDto { ProductId = productId, Quantity = _faker.Random.Int(1, 20) };
        var result = await _validator.ValidateAsync(item);

        Assert.True(result.IsValid);
    }
}
