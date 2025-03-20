using MediatR;
using Module.Users.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Users.Application.Commands.Auth.LoginCommand;

public record LoginCommand(string Email, string Password) : IRequest<ServiceResult<LoginResponseDto>>;