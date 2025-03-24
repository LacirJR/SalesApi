using MediatR;
using Module.Sales.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Sales.Application.Queries.GetSaleByIdQuery;

public record GetSaleByIdQuery(Guid SaleId) : IRequest<ServiceResult<SaleResponseDto>>;