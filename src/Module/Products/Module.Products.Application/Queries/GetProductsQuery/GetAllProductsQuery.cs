using MediatR;
using Module.Products.Application.Dtos;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Products.Application.Queries.GetProductsQuery;

public class GetAllProductsQuery :  IRequest<ServiceResult<PaginatedList<ProductResponseDto>>>
{
    public string? Filter { get; set; }
    public string? OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;

}