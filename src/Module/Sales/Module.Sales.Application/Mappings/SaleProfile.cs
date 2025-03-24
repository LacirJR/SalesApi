using AutoMapper;
using Module.Sales.Application.Dtos;
using Module.Sales.Domain.Entities;

namespace Module.Sales.Application.Mappings;

public class SaleProfile :  Profile
{
    public SaleProfile()
    {
        CreateMap<Sale, SaleResponseDto>()
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.TotalValue))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<SaleItem, SaleItemDto>()
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => (double)src.DiscountPercentage))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));
    }
}