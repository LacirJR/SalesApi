using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Carts.Application.Commands.UpdateCartCommand;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Domain.Entities;
using NSubstitute;
using Shared.Application.Dtos;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.Domain.Common;
using Xunit;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationException = FluentValidation.ValidationException;

namespace Module.Carts.Tests.Unit.Handlers;

public class UpdateCartCommandHandlerTests
{
    private readonly Faker _faker;
    private readonly ICartRepository _cartRepository;
    private readonly IDiscountRuleRepository _discountRuleRepository;
    private readonly ISharedProductService _sharedProductService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdateCartCommand> _validator;
    private readonly UpdateCartCommandHandler _handler;

    public UpdateCartCommandHandlerTests()
    {
        _faker = new Faker();
        _cartRepository = Substitute.For<ICartRepository>();
        _discountRuleRepository = Substitute.For<IDiscountRuleRepository>();
        _sharedProductService = Substitute.For<ISharedProductService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _validator = Substitute.For<IValidator<UpdateCartCommand>>();

        _handler = new UpdateCartCommandHandler(
            _cartRepository,
            _discountRuleRepository,
            _sharedProductService,
            _unitOfWork,
            _mapper,
            _validator
        );
    }
    
    [Fact]
    public async Task Should_Throw_ValidationException_When_Command_Is_Invalid()
    {
        var command = new UpdateCartCommand(Guid.Empty, Guid.NewGuid(), _faker.Date.Recent(),new List<CartItemDto>());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("CartId", "Required") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Should_Fail_When_Cart_Not_Found()
    {
        var command = new UpdateCartCommand(_faker.Random.Guid(), Guid.NewGuid(), _faker.Date.Recent(), new List<CartItemDto>());

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound.Type, result.Error?.Type);
    }
    
    [Fact]
    public async Task Should_Fail_When_Product_Not_Found()
    {
        var cartId = _faker.Random.Guid();
        var productId = _faker.Random.Guid();

        var command = new UpdateCartCommand(cartId,Guid.NewGuid(), _faker.Date.Recent(), new List<CartItemDto>
        {
            new(productId, 2, 12m)
        });

        _validator.ValidateAsync(command, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(new Cart(_faker.Random.Guid(), _faker.Date.Past()));

        _sharedProductService.GetProductByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns((SharedProductDto?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal("NotFound", result.Error?.Type);
        Assert.Contains(productId.ToString(), result.Error?.Detail);
    }
    
    
}