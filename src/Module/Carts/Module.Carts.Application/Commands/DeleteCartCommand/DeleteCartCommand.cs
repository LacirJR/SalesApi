using MediatR;
using Module.Carts.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Carts.Application.Commands.DeleteCartCommand;

public record DeleteCartCommand(Guid CartId) : IRequest<ServiceResult<DeleteCartDto>>;