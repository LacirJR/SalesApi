using Shared.Domain.Common.Events;

namespace Shared.Domain.Common;

public abstract record DomainEvent(Guid? GuidId , int? Id) : IDomainEvent;
