using MediatR;
using Module.Sales.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Sales.Application.Queries.GetSaleByNumberQuery;

public record GetSaleByNumberQuery(long Number) : IRequest<ServiceResult<SaleResponseDto>>;
