using MediatR;
using Microsoft.Extensions.Logging;
using Module.Sales.Domain.Events;

namespace Module.Sales.Application.Events.Handlers;

public class SaleCanceledEventHandler: INotificationHandler<SaleCanceledEvent>
{
    private readonly ILogger<SaleCanceledEventHandler> _logger;

    public SaleCanceledEventHandler(ILogger<SaleCanceledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCanceledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sale with ID {SaleId} was canceled.", notification.SaleId);
        return Task.CompletedTask;
    }
}