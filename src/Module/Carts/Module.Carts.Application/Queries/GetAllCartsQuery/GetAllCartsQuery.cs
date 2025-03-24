using MediatR;
using Module.Carts.Application.Dtos;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Carts.Application.Queries.GetAllCartsQuery;

public record GetAllCartsQuery(string? Filter, string? OrderBy, int Page = 1, int Size = 10) : IRequest<ServiceResult<PaginatedList<CartResponseDto>>>;
