using Shared.Domain.Common;

namespace Module.Sales.Domain.Events;

public record ItemCanceledEvent(Guid SaleId, Guid ProductId) : BaseEvent;