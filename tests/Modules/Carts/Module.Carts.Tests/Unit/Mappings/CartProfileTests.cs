using AutoMapper;
using Bogus;
using Module.Carts.Application.Dtos;
using Module.Carts.Application.Mappings;
using Module.Carts.Domain.Entities;
using Xunit;

namespace Module.Carts.Tests.Unit.Mappings;

public class CartProfileTests
{
    private readonly IMapper _mapper;
    private readonly Faker _faker;

    public CartProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<CartProfile>());
        _mapper = config.CreateMapper();
        _faker = new Faker();
    }

    [Fact]
    public void Should_Map_Cart_To_CartResponseDto()
    {
        var cart = new Cart(_faker.Random.Guid(), _faker.Date.Past());
        
        var cartItem1 = new CartItem(cart.Id, _faker.Random.Guid(), 2, 10);
        var cartItem2 = new CartItem(cart.Id, _faker.Random.Guid(), 1, 15);
        
        cart.AddItem(cartItem1);
        cart.AddItem(cartItem2);


        var result = _mapper.Map<CartResponseDto>(cart);

        Assert.Equal(cart.Id, result.Id);
        Assert.Equal(cart.UserId, result.UserId);
        Assert.Equal(cart.Date, result.Date);
        Assert.Equal(35, result.TotalPrice); 
    }

    [Fact]
    public void Should_Map_CartItem_To_CartItemDto()
    {
        var cartItem = new CartItem(_faker.Random.Guid(), _faker.Random.Guid(), _faker.Random.Int(1, 10), _faker.Finance.Amount());

        var result = _mapper.Map<CartItemDto>(cartItem);

        Assert.Equal(cartItem.ProductId, result.ProductId);
        Assert.Equal(cartItem.Quantity, result.Quantity);
    }

    [Fact]
    public void Should_Map_DiscountRule_To_DiscountRuleDto()
    {
        var rule = new DiscountRule(_faker.Random.Int(1, 5), _faker.Random.Int(6, 10), _faker.Random.Int(10, 30));

        var result = _mapper.Map<DiscountRuleDto>(rule);

        Assert.Equal(rule.MinQuantity, result.MinQuantity);
        Assert.Equal(rule.MaxQuantity, result.MaxQuantity);
        Assert.Equal(rule.DiscountPercentage, result.DiscountPercentage);
    }
    
}