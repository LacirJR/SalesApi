namespace Module.Users.Application.Dtos;

public record UpdateUserDto(
    string Email,
    string Username,
    string Password,
    NameDto Name,
    AddressDto Address,
    string Phone,
    string Status,
    string Role);