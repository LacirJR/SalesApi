using MediatR;
using Module.Users.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Users.Application.Commands.UpdateUserCommand;

public record UpdateUserCommand(Guid Id,
    string Email,
    string Username,
    string Password,
    NameDto Name,
    AddressDto Address,
    string Phone,
    string Status,
    string Role) : IRequest<ServiceResult<UserResponseDto>>;