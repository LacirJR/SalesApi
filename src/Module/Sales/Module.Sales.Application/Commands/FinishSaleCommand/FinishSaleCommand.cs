using MediatR;
using Module.Sales.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Sales.Application.Commands.FinishSaleCommand;

public record FinishSaleCommand(Guid SaleId) : IRequest<ServiceResult<SaleResponseDto>>;
