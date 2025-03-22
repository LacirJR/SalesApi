using AutoMapper;
using Bogus;
using FluentValidation;
using FluentValidation.Results;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Interfaces.Persistence;
using Module.Carts.Application.Queries.GetCartByIdQuery;
using Module.Carts.Domain.Entities;
using NSubstitute;
using Shared.Domain.Common;
using Xunit;
using ValidationResult = FluentValidation.Results.ValidationResult;
using ValidationException = FluentValidation.ValidationException;

namespace Module.Carts.Tests.Unit.Handlers;

public class GetCartByIdQueryHandlerTests
{
    private readonly Faker _faker;
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<GetCartByIdQuery> _validator;
    private readonly GetCartByIdQueryHandler _handler;

    public GetCartByIdQueryHandlerTests()
    {
        _faker = new Faker();
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _validator = Substitute.For<IValidator<GetCartByIdQuery>>();

        _handler = new GetCartByIdQueryHandler(_cartRepository, _mapper, _validator);
    }
    
    [Fact]
    public async Task Should_Throw_ValidationException_When_Query_Is_Invalid()
    {
        var query = new GetCartByIdQuery(Guid.Empty);

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("CartId", "CartId is required.") }));

        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(query, CancellationToken.None));
    }
    
    [Fact]
    public async Task Should_Fail_When_Cart_Not_Found()
    {
        var cartId = _faker.Random.Guid();
        var query = new GetCartByIdQuery(cartId);

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns((Cart?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(ServiceError.NotFound.Type, result.Error?.Type);
    }
    
    [Fact]
    public async Task Should_Return_Cart_Successfully()
    {
        var cartId = _faker.Random.Guid();
        var cart = new Cart(_faker.Random.Guid(), _faker.Date.Past());

        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(cart, cartId);

        var query = new GetCartByIdQuery(cartId);

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(cart);

        _mapper.Map<CartResponseDto>(cart)
            .Returns(new CartResponseDto { Id = cartId });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(cartId, result.Data.Id);
    }
    
    [Fact]
    public async Task Should_Return_Cart_By_Date()
    {
        var targetDate = _faker.Date.Past();
        var cart = new Cart(_faker.Random.Guid(), targetDate);
        var query = new GetCartByIdQuery(cart.Id);
        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>())
            .Returns(cart);

        _mapper.Map<CartResponseDto>(cart)
            .Returns(new CartResponseDto { Id = cart.Id, Date = cart.Date });

  
        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(targetDate, result.Data.Date);
    }
    
    [Fact]
    public async Task Should_Return_Cart_By_User()
    {
        var userId = _faker.Random.Guid();
        var cart = new Cart(userId, _faker.Date.Past());
        var query = new GetCartByIdQuery(cart.Id);

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        _cartRepository.GetByIdAsync(cart.Id, Arg.Any<CancellationToken>())
            .Returns(cart);

        _mapper.Map<CartResponseDto>(cart)
            .Returns(new CartResponseDto { Id = cart.Id, UserId = userId });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(userId, result.Data.UserId);
    }
    
    
    [Fact]
    public async Task Should_Return_Cart_With_Multiple_Filters()
    {
        var userId = _faker.Random.Guid();
        var targetDate = _faker.Date.Past();
        var cartId = _faker.Random.Guid();
        var cart = new Cart(userId, targetDate);

        cart.Id = cartId;
        
        var query = new GetCartByIdQuery(cartId);

        _validator.ValidateAsync(query, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(cart, cartId);

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>())
            .Returns(cart);

        _mapper.Map<CartResponseDto>(cart)
            .Returns(new CartResponseDto { Id = cartId, UserId = userId, Date = targetDate });

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(cartId, result.Data.Id);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Equal(targetDate, result.Data.Date);
    }

    
}