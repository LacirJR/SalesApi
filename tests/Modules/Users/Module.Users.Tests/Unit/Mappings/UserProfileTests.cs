using System.Globalization;
using AutoMapper;
using Module.Users.Application.Dtos;
using Module.Users.Application.Mappings;
using Module.Users.Domain.Entities;
using Module.Users.Domain.Enums;
using Module.Users.Domain.ValueObjects;
using Shared.Domain.Common.Enums;
using Xunit;

namespace Module.Users.Tests.Unit.Mappings;

public class UserProfileTests
{
    private readonly IMapper _mapper;

    public UserProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>());
        _mapper = config.CreateMapper();
    }
    
    [Fact]
    public void Should_Map_User_To_CreateUserResponseDto()
    {
        var user = new User(
            "test@example.com", "testuser", "hashedPassword",
            new Name("John", "Doe"),
            new Address("City", "Street", 100, "123456", new Geolocation("12.34", "56.78")),
            "123456789",
            UserRole.Admin, UserStatus.Active
        );

        var result = _mapper.Map<UserResponseDto>(user);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Name.Firstname, result.Name.Firstname);
        Assert.Equal(user.Name.Lastname, result.Name.Lastname);
        Assert.Equal(user.Address.City, result.Address.City);
        Assert.Equal(user.Address.Street, result.Address.Street);
        Assert.Equal(user.Address.Number, result.Address.Number);
        Assert.Equal(user.Address.Zipcode, result.Address.Zipcode);
        Assert.Equal(user.Address.Geolocation.Lat, result.Address.Geolocation.Lat);
        Assert.Equal(user.Address.Geolocation.Long, result.Address.Geolocation.Long);
        Assert.Equal(user.Phone, result.Phone);
        Assert.Equal(user.Status.ToString(), result.Status);
        Assert.Equal(user.Role.ToString(), result.Role);
    }
    
}