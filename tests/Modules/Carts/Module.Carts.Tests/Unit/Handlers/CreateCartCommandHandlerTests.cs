using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Carts.Application.Commands.CreateCartCommand;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Application.Validations;
using Module.Carts.Domain.Entities;
using Module.Products.Application.Dtos;
using NSubstitute;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Xunit;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationException = FluentValidation.ValidationException;

namespace Module.Carts.Tests.Unit.Handlers;

public class CreateCartCommandHandlerTests
{
    private readonly Faker _faker;
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCartCommand> _validator;
    private readonly IDiscountRuleRepository _discountRuleRepository;
    private readonly ISharedProductService _sharedProductService;
    private readonly CreateCartCommandHandler _handler;

    public CreateCartCommandHandlerTests()
    {
        _faker = new Faker();
        _cartRepository = Substitute.For<ICartRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _validator = Substitute.For<IValidator<CreateCartCommand>>();
        _discountRuleRepository = Substitute.For<IDiscountRuleRepository>();
        _sharedProductService = Substitute.For<ISharedProductService>();

        _handler = new CreateCartCommandHandler(
            _cartRepository, _unitOfWork, _mapper, _validator,
            _discountRuleRepository, _sharedProductService);
    }
    
    [Fact]
    public async Task Should_Throw_ValidationException_When_Command_Is_Invalid()
    {
        var command = new CreateCartCommand(Guid.Empty, default, new List<CartItemDto>());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("UserId", "User ID is required.") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Should_Fail_When_User_Already_Has_Active_Cart()
    {
        var userId = _faker.Random.Guid();
        var command = new CreateCartCommand(userId, _faker.Date.Past(), new List<CartItemDto>());
        var existingCart = new Cart(userId, command.Date);

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _cartRepository.GetCartByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(existingCart);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("ActiveCartExists", result.Error?.Error);
    }
    
    [Fact]
    public async Task Should_Fail_When_Product_Not_Found()
    {
        var userId = _faker.Random.Guid();
        var productId = _faker.Random.Guid();

        var command = new CreateCartCommand(userId, _faker.Date.Past(), new List<CartItemDto>
        {
            new(productId, _faker.Random.Int(1, 20), _faker.Random.Decimal(1, 1000))
        });

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _cartRepository.GetCartByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        _sharedProductService.GetProductByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns((SharedProductDto?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("NotFound", result.Error?.Type);
    }
    
    [Fact]
    public async Task Should_Create_Cart_Successfully()
    {
        var userId = _faker.Random.Guid();
        var productId = _faker.Random.Guid();
        var price = _faker.Random.Decimal(10, 100);

        var command = new CreateCartCommand(userId, _faker.Date.Recent(), new List<CartItemDto>
        {
            new(productId, _faker.Random.Int(1, 20), _faker.Random.Decimal(1, 1000))
        });

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _cartRepository.GetCartByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        _sharedProductService.GetProductByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(new SharedProductDto { Id = productId, Price = price });

        _discountRuleRepository.GetActiveRulesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<DiscountRule>
            {
                new(4, 9, 10) 
            });

        _mapper.Map<CartResponseDto>(Arg.Any<Cart>())
            .Returns(new CartResponseDto { Id = Guid.NewGuid(), Products = new List<CartItemDto>() });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
    }

}