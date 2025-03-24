using Shared.Domain.Common;

namespace Module.Sales.Domain.Events;

public record SaleCanceledEvent(Guid SaleId) : BaseEvent;