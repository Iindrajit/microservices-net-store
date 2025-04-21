using Ordering.Domain.Abstractions;

namespace Ordering.Application.Notifications
{
    // Represents a notification that wraps a domain event, making it compatible with MediatR's INotification system.
    public class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent // Ensures that TDomainEvent is constrained to implement IDomainEvent, which represents a domain event.
    {
        // The domain event being wrapped by this notification.
        // This will be published via MediatR to notify handlers that an event has occurred.
        public TDomainEvent DomainEvent { get; }

        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
