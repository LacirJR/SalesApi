using MediatR;
using Module.Products.Application.Dtos;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Products.Application.Queries.GetProductsByCategoryNameQuery;

public record GetProductsByCategoryNameQuery(string CategoryName, string? Order, int Page = 1, int Size = 10) : IRequest<ServiceResult<PaginatedList<ProductResponseDto>>>;