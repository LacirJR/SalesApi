using Module.Users.Application.Dtos;
using Module.Users.Domain.Entities;

namespace Module.Users.Application.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    LoginResponseDto GenerateToken(User user);
}