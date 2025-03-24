using MassTransit;
using Microsoft.Extensions.Logging;
using Module.Carts.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.IntegrationEvents.Contracts.Products;

namespace Module.Carts.Infrastructure.Consumers;

public class ProductDeletedCartConsumer : IConsumer<ProductDeletedIntegrationEvent>
{
    private readonly ISharedCartService _sharedCartService;
    private readonly ICartUnitOfWork _unitOfWork;
    private readonly ILogger<ProductDeletedCartConsumer> _logger;

    public ProductDeletedCartConsumer(ISharedCartService sharedCartService, ICartUnitOfWork unitOfWork, ILogger<ProductDeletedCartConsumer> logger)
    {
        _sharedCartService = sharedCartService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductDeletedIntegrationEvent> context)
    {
        await _sharedCartService.RemoveProductByIdAsync(context.Message.ProductId, context.CancellationToken);
        await _unitOfWork.CommitAsync();
        
        _logger.LogInformation($"Product {context.Message.ProductId} has been deleted of active carts");
    }
}