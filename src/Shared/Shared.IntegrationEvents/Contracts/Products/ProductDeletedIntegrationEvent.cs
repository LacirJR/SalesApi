namespace Shared.IntegrationEvents.Contracts.Products;

public class ProductDeletedIntegrationEvent
{
    public Guid ProductId { get; set; }

    public ProductDeletedIntegrationEvent(Guid productId)
    {
        ProductId = productId;
    }
}