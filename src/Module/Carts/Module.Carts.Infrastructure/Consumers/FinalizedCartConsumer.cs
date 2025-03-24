using MassTransit;
using Microsoft.Extensions.Logging;
using Module.Carts.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.IntegrationEvents.Contracts.Carts;

namespace Module.Carts.Infrastructure.Consumers;

public class FinalizedCartConsumer : IConsumer<FinalizedCartIntegrationEvent>
{
    private readonly ISharedCartService _sharedCartService;
    private readonly ICartUnitOfWork _unitOfWork;
    private readonly ILogger<FinalizedCartConsumer> _logger;

    public FinalizedCartConsumer(ISharedCartService sharedCartService, ICartUnitOfWork unitOfWork, ILogger<FinalizedCartConsumer> logger)
    {
        _sharedCartService = sharedCartService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<FinalizedCartIntegrationEvent> context)
    {
        await _sharedCartService.FinalizeCartAsync(context.Message.CartId, context.CancellationToken);
        await _unitOfWork.CommitAsync(context.CancellationToken);
        _logger.Log(LogLevel.Information, $"Cart {context.Message.CartId} finalized");
    }
}