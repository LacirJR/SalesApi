using Shared.Domain.Common;

namespace Module.Sales.Domain.Events;

public record SaleCreatedEvent(Guid SaleId, Guid CartId) : BaseEvent;