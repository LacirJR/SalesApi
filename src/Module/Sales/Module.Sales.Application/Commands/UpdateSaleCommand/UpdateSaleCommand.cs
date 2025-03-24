using MediatR;
using Module.Sales.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Sales.Application.Commands.UpdateSaleCommand;

public record UpdateSaleCommand(Guid SaleId, string Branch, DateTime Date) : IRequest<ServiceResult<SaleResponseDto>>;
