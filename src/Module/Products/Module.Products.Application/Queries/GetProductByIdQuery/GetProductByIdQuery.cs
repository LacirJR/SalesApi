using MediatR;
using Module.Products.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Products.Application.Queries.GetProductByIdQuery;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ServiceResult<ProductResponseDto>>;