using MediatR;
using Module.Sales.Application.Dtos;
using Shared.Domain.Common;
using Shared.Infrastructure.Common;

namespace Module.Sales.Application.Queries.GetAllSalesQuery;

public record GetAllSalesQuery(string? Filter, string? OrderBy, int Page, int Size)
    : IRequest<ServiceResult<PaginatedList<SaleResponseDto>>>;