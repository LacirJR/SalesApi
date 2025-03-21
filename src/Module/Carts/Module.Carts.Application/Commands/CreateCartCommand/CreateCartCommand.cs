using MediatR;
using Module.Carts.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Carts.Application.Commands.CreateCartCommand;

public record CreateCartCommand() : IRequest<ServiceResult<CartResponseDto>>;
