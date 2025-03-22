using Bogus;
using FluentValidation.TestHelper;
using Module.Carts.Application.Commands.UpdateCartCommand;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Validations;
using NSubstitute;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Services;
using Xunit;

namespace Module.Carts.Tests.Unit.Validations;

public class UpdateCartValidatorTests
{
    private readonly Faker _faker;
    private readonly ISharedUserService _sharedUserService;
    private readonly ISharedProductService _sharedProductService;
    private readonly UpdateCartValidator _validator;

    public UpdateCartValidatorTests()
    {
        _faker = new Faker();
        _sharedUserService = Substitute.For<ISharedUserService>();
        _sharedProductService = Substitute.For<ISharedProductService>();

        _validator = new UpdateCartValidator(_sharedProductService, _sharedUserService);
    }

    [Fact]
    public async Task Should_Fail_When_CartId_Is_Empty()
    {
        var command = new UpdateCartCommand(Guid.Empty, _faker.Random.Guid(), DateTime.UtcNow, new List<CartItemDto>());
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.CartId);
    }

    [Fact]
    public async Task Should_Fail_When_Date_Is_In_Future()
    {
        var command = new UpdateCartCommand(_faker.Random.Guid(), _faker.Random.Guid(), DateTime.UtcNow.AddDays(1), new List<CartItemDto>());
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Date);
    }

    [Fact]
    public async Task Should_Fail_When_User_Does_Not_Exist()
    {
        var userId = _faker.Random.Guid();
        _sharedUserService.UserExistsAsync(userId, Arg.Any<CancellationToken>()).Returns(false);

        var command = new UpdateCartCommand(_faker.Random.Guid(), userId, DateTime.UtcNow, new List<CartItemDto>());
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var userId = _faker.Random.Guid();
        var productId = _faker.Random.Guid();
        var cartId = _faker.Random.Guid();

        var products = new List<CartItemDto>
        {
            new CartItemDto { ProductId = productId, Quantity = 5 }
        };

        _sharedUserService.UserExistsAsync(userId, Arg.Any<CancellationToken>()).Returns(true);
        _sharedProductService.GetProductByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(new SharedProductDto());

        var command = new UpdateCartCommand(cartId, userId, _faker.Date.Recent(), products);

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }
}
