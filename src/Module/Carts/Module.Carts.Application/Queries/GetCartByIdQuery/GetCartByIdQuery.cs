using MediatR;
using Module.Carts.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Carts.Application.Queries.GetCartByIdQuery;

public record GetCartByIdQuery(Guid CartId) : IRequest<ServiceResult<CartResponseDto>>;