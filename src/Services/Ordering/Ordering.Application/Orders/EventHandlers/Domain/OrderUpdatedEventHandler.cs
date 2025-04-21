//namespace Ordering.Application.Orders.EventHandlers.Domain;
//public class OrderUpdatedEventHandler(ILogger<OrderUpdatedEventHandler> logger)
//    : INotificationHandler<OrderUpdatedEvent>
//{
//    public Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
//    {
//        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
//        return Task.CompletedTask;
//    }
//}

using Ordering.Application.Notifications;

namespace Ordering.Application.Orders.EventHandlers.Domain;
public class OrderUpdatedEventHandler(ILogger<OrderUpdatedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<OrderUpdatedEvent>>
{
    public Task Handle(DomainEventNotification<OrderUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        logger.LogInformation("Domain Event handled: {DomainEvent}", domainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
