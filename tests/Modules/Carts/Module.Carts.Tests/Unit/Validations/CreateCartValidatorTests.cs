using Bogus;
using FluentValidation.TestHelper;
using Module.Carts.Application.Commands.CreateCartCommand;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Validations;
using NSubstitute;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Services;
using Xunit;

namespace Module.Carts.Tests.Unit.Validations;

public class CreateCartValidatorTests
{
    private readonly Faker _faker;
    private readonly ISharedUserService _sharedUserService;
    private readonly ISharedProductService _sharedProductService;
    private readonly CreateCartValidator _validator;

    public CreateCartValidatorTests()
    {
        _faker = new Faker();
        _sharedUserService = Substitute.For<ISharedUserService>();
        _sharedProductService = Substitute.For<ISharedProductService>();

        _validator = new CreateCartValidator(_sharedUserService, _sharedProductService);
    }

    [Fact]
    public async Task Should_Fail_When_UserId_Is_Empty()
    {
        var command = new CreateCartCommand(Guid.Empty, _faker.Date.Past(), new List<CartItemDto>());
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_Fail_When_User_Does_Not_Exist()
    {
        var userId = _faker.Random.Guid();
        _sharedUserService.UserExistsAsync(userId, Arg.Any<CancellationToken>()).Returns(false);

        var command = new CreateCartCommand(userId, _faker.Date.Past(), new List<CartItemDto>());
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public async Task Should_Fail_When_Products_Are_Empty()
    {
        var userId = _faker.Random.Guid();
        _sharedUserService.UserExistsAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        var command = new CreateCartCommand(userId, _faker.Date.Past(), new List<CartItemDto>());
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Products);
    }

    [Fact]
    public async Task Should_Pass_When_Valid()
    {
        var userId = _faker.Random.Guid();
        var products = new List<CartItemDto>
        {
            new CartItemDto { ProductId = _faker.Random.Guid(), Quantity = _faker.Random.Int(1, 20) }
        };

        _sharedUserService.UserExistsAsync(userId, Arg.Any<CancellationToken>()).Returns(true);
        _sharedProductService.GetProductByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(new SharedProductDto());

        var command = new CreateCartCommand(userId, _faker.Date.Past(), products);
        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }
}
