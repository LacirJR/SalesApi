using MediatR;
using Module.Users.Application.Dtos;
using Module.Users.Domain.Enums;
using Shared.Domain.Common;
using Shared.Domain.Common.Enums;

namespace Module.Users.Application.Commands.CreateUserCommand;

public record CreateUserCommand(string Email,
    string Username,
    string Password,
    NameDto Name,
    AddressDto Address,
    string Phone,
    string Status,
    string Role) : IRequest<ServiceResult<UserResponseDto>>;