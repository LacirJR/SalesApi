using MediatR;
using Microsoft.Extensions.Logging;
using Module.Sales.Domain.Events;

namespace Module.Sales.Application.Events.Handlers;

public class SaleFinalizedEventHandler: INotificationHandler<SaleFinalizedEvent>
{
    private readonly ILogger<SaleFinalizedEventHandler> _logger;

    public SaleFinalizedEventHandler(ILogger<SaleFinalizedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleFinalizedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sale with ID {SaleId} was finalized.", notification.SaleId);
        return Task.CompletedTask;
    }
}