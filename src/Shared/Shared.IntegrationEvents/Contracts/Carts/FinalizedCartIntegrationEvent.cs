namespace Shared.IntegrationEvents.Contracts.Carts;

public class FinalizedCartIntegrationEvent
{
    public Guid CartId { get; set; }

    public FinalizedCartIntegrationEvent(Guid cartId)
    {
        CartId = cartId;
    }
}