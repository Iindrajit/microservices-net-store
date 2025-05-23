﻿//using MassTransit;
//using Microsoft.FeatureManagement;

//namespace Ordering.Application.Orders.EventHandlers.Domain;
//public class OrderCreatedEventHandler
//    (IPublishEndpoint publishEndpoint, IFeatureManager featureManager, ILogger<OrderCreatedEventHandler> logger)
//    : INotificationHandler<OrderCreatedEvent>
//{
//    public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
//    {
//        logger.LogInformation("Domain Event handled: {DomainEvent}", domainEvent.GetType().Name);

//        if (await featureManager.IsEnabledAsync("OrderFullfilment"))
//        {
//            var orderCreatedIntegrationEvent = domainEvent.order.ToOrderDto();
//            await publishEndpoint.Publish(orderCreatedIntegrationEvent, cancellationToken);
//        }
//    }
//}

using MassTransit;
using Microsoft.FeatureManagement; 
using Ordering.Application.Notifications;

namespace Ordering.Application.Orders.EventHandlers.Domain;
public class OrderCreatedEventHandler
    (IPublishEndpoint publishEndpoint, IFeatureManager featureManager, ILogger<OrderCreatedEventHandler> logger)
    : INotificationHandler<DomainEventNotification<OrderCreatedEvent>>
{

    public async Task Handle(DomainEventNotification<OrderCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        logger.LogInformation("Domain Event handled: {DomainEvent}", domainEvent.GetType().Name);

        if (await featureManager.IsEnabledAsync("OrderFullfilment"))
        {
            var orderCreatedIntegrationEvent = domainEvent.order.ToOrderDto();
            await publishEndpoint.Publish(orderCreatedIntegrationEvent, cancellationToken);
        }
    }
}