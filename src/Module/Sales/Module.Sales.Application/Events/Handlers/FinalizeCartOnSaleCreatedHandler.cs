using MediatR;
using Microsoft.Extensions.Logging;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Events;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;

namespace Module.Sales.Application.Events.Handlers;

public class FinalizeCartOnSaleCreatedHandler : INotificationHandler<SaleCreatedEvent>
{
    private readonly ISharedCartService _sharedCartService;
    private readonly ISaleUnitOfWork _unitOfWork;
    private readonly ILogger<FinalizeCartOnSaleCreatedHandler> _logger;

    public FinalizeCartOnSaleCreatedHandler(ISharedCartService sharedCartService, ISaleUnitOfWork unitOfWork, ILogger<FinalizeCartOnSaleCreatedHandler> logger)
    {
        _sharedCartService = sharedCartService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, "Handling sale creation event");
        await _sharedCartService.FinalizeCartAsync(notification.CartId, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        _logger.Log(LogLevel.Information, "Sale creation event sent");
    }
}