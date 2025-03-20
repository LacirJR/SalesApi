using MediatR;

namespace Shared.Domain.Common.Events;

public interface IDomainEvent : INotification
{
    Guid? GuidId { get; }
    int? Id { get; }
}