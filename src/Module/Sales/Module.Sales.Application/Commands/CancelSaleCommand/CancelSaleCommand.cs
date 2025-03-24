using MediatR;
using Module.Sales.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Sales.Application.Commands.CancelSaleCommand;

public record CancelSaleCommand(Guid SaleId) : IRequest<ServiceResult<SaleResponseDto>>;
