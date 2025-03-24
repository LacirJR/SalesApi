using MediatR;
using Module.Carts.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Carts.Application.Commands.CreateCartCommand;

public record CreateCartCommand(Guid UserId, DateTime Date, List<CartItemDto> Products) : IRequest<ServiceResult<CartResponseDto>>;
