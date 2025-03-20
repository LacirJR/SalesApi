namespace Module.Users.Application.Dtos;

public record LoginResponseDto(string Token, DateTime? Expiration);