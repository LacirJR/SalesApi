using AutoMapper;
using Module.Carts.Application.Dtos;
using Module.Carts.Domain.Entities;

namespace Module.Carts.Application.Mappings;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<Cart, CartResponseDto>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => Math.Round(src.Products.Sum(i => i.GetTotalPrice()), 2, MidpointRounding.AwayFromZero)));

        CreateMap<CartItem, CartItemDto>();
        
        CreateMap<DiscountRule, DiscountRuleDto>();
    }
}