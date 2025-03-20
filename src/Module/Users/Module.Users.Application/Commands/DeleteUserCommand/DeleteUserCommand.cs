using MediatR;
using Module.Users.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Users.Application.Commands.DeleteUserCommand;

public record DeleteUserCommand(Guid Id) : IRequest<ServiceResult<UserResponseDto>>;