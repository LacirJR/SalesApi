using Bogus;
using Module.Sales.Application.Commands.CreateFromCartCommand;
using Module.Sales.Application.Validations;
using NSubstitute;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Services;
using Xunit;

namespace Module.Sales.Tests.Unit.Validations;

public class CreateSaleFromCartValidatorTests
{
    private readonly ISharedCartService _sharedCartService;
    private readonly CreateSaleFromCartValidator _validator;
    private readonly Faker _faker;

    public CreateSaleFromCartValidatorTests()
    {
        _sharedCartService = Substitute.For<ISharedCartService>();
        _validator = new CreateSaleFromCartValidator(_sharedCartService);
        _faker = new Faker();
    }

    [Fact]
    public async Task Should_Pass_When_Command_Is_Valid()
    {
        var command = new CreateSaleFromCartCommand(
            _faker.Random.Guid(),
            _faker.Company.CompanyName()
        );

        _sharedCartService.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(new SharedCartDto());

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Should_Fail_When_CartId_Is_Empty()
    {
        var command = new CreateSaleFromCartCommand(Guid.Empty, _faker.Company.CompanyName());

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.CartId));
    }

    [Fact]
    public async Task Should_Fail_When_Cart_Not_Found()
    {
        var command = new CreateSaleFromCartCommand(_faker.Random.Guid(), _faker.Company.CompanyName());

        _sharedCartService.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns((SharedCartDto?)null);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.CartId));
    }

    [Fact]
    public async Task Should_Fail_When_Branch_Is_Empty()
    {
        var command = new CreateSaleFromCartCommand(_faker.Random.Guid(), "");

        _sharedCartService.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(new SharedCartDto());

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Branch));
    }

    [Fact]
    public async Task Should_Fail_When_Branch_Exceeds_Max_Length()
    {
        var longBranch = new string('A', 101);
        var command = new CreateSaleFromCartCommand(_faker.Random.Guid(), longBranch);

        _sharedCartService.GetCartByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns(new SharedCartDto());

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Branch));
    }
}
