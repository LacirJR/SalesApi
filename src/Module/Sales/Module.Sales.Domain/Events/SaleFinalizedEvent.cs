using Shared.Domain.Common;

namespace Module.Sales.Domain.Events;

public record SaleFinalizedEvent(Guid SaleId) : BaseEvent;