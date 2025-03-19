namespace Module.Users.Application.Dtos;

public sealed record AddressDto(
    string City,
    string Street,
    int Number,
    string Zipcode,
    GeolocationDto Geolocation
);