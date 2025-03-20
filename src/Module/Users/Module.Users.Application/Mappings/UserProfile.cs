using AutoMapper;
using Module.Users.Application.Dtos;
using Module.Users.Domain.Entities;

namespace Module.Users.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new NameDto(src.Name.Firstname, src.Name.Lastname)))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new AddressDto(
                src.Address.City,
                src.Address.Street,
                src.Address.Number,
                src.Address.Zipcode,
                new GeolocationDto(src.Address.Geolocation.Lat, src.Address.Geolocation.Long)
            )));
    }
}