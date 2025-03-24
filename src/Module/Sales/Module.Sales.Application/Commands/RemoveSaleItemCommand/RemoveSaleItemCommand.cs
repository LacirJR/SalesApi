using MediatR;
using Module.Sales.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Sales.Application.Commands.RemoveSaleItemCommand;

public record RemoveSaleItemCommand(Guid SaleId, Guid ProductId) : IRequest<ServiceResult<SaleResponseDto>>;
