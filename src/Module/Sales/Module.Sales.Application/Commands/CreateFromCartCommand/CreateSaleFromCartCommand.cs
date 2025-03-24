using MediatR;
using Module.Sales.Application.Dtos;
using Shared.Domain.Common;

namespace Module.Sales.Application.Commands.CreateFromCartCommand;

public record CreateSaleFromCartCommand(Guid CartId, string Branch)
    : IRequest<ServiceResult<SaleResponseDto>>;