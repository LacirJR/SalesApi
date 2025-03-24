using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Module.Sales.Application.Interfaces.Persistence;
using Module.Sales.Domain.Events;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.IntegrationEvents.Contracts.Carts;

namespace Module.Sales.Application.Events.Handlers;

public class FinalizeCartOnSaleCreatedHandler : INotificationHandler<SaleCreatedEvent>
{
    private readonly ILogger<FinalizeCartOnSaleCreatedHandler> _logger;
    private readonly IPublishEndpoint _publisher;

    public FinalizeCartOnSaleCreatedHandler(IPublishEndpoint publisher, ILogger<FinalizeCartOnSaleCreatedHandler> logger)
    {
        _logger = logger;
        _publisher = publisher;
    }

    public async Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Log(LogLevel.Information, "Handling sale creation event");
        await _publisher.Publish(new FinalizedCartIntegrationEvent(notification.CartId));
        _logger.Log(LogLevel.Information, "Sale creation event sent");
    }
}