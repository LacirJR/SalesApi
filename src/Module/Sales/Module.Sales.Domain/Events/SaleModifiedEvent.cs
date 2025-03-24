using Shared.Domain.Common;

namespace Module.Sales.Domain.Events;

public record SaleModifiedEvent(Guid SaleId) : BaseEvent;