using MediatR;
using Microsoft.Extensions.Logging;
using Module.Sales.Domain.Events;

namespace Module.Sales.Application.Events.Handlers;

public class ItemCanceledEventHandler : INotificationHandler<ItemCanceledEvent>
{
    
    private readonly ILogger<ItemCanceledEventHandler> _logger;

    public ItemCanceledEventHandler(ILogger<ItemCanceledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ItemCanceledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Item with ProductId {ProductId} was removed from Sale ID {SaleId}.", 
            notification.ProductId, notification.SaleId);
        
        return Task.CompletedTask;
    }
}