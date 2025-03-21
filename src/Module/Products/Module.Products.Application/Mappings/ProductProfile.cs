using AutoMapper;
using Module.Products.Application.Commands.CreateProductCommand;
using Module.Products.Application.Dtos;
using Module.Products.Domain.Entities;

namespace Module.Products.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new RatingDto(src.Rating.Rate, src.Rating.Count)));
    }
}