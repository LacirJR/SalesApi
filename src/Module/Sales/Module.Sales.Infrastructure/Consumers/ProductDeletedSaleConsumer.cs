using MassTransit;
using Microsoft.Extensions.Logging;
using Module.Sales.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Persistence;
using Shared.Application.Interfaces.Services;
using Shared.IntegrationEvents.Contracts.Products;

namespace Module.Sales.Infrastructure.Consumers;

public class ProductDeletedSaleConsumer : IConsumer<ProductDeletedIntegrationEvent>
{
    private readonly ISharedSaleService _sharedSaleService;
    private readonly ISaleUnitOfWork _unitOfWork;
    private readonly ILogger<ProductDeletedSaleConsumer> _logger;

    public ProductDeletedSaleConsumer(ISharedSaleService sharedSaleService, ISaleUnitOfWork unitOfWork,
        ILogger<ProductDeletedSaleConsumer> logger)
    {
        _sharedSaleService = sharedSaleService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductDeletedIntegrationEvent> context)
    {
        await _sharedSaleService.RemoveProductByIdAsync(context.Message.ProductId, context.CancellationToken);
        await _unitOfWork.CommitAsync();
        _logger.LogInformation($"Product {context.Message.ProductId} has been deleted of active sales");
    }
}