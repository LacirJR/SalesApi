using MediatR;
using Module.Carts.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Carts.Application.Commands.UpdateCartCommand;

public record UpdateCartCommand(
    Guid CartId,
    Guid UserId,
    DateTime Date,
    List<CartItemDto> Products
) : IRequest<ServiceResult<CartResponseDto>>;