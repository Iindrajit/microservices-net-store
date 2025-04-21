using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Ordering.Application.Notifications;

namespace Ordering.Infrastructure.Data.Interceptors;
public class DispatchDomainEventsInterceptor(IMediator mediator)
    : SaveChangesInterceptor
{
    //public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    //{
    //    DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
    //    return base.SavingChanges(eventData, result);
    //}

    //public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    //{
    //    await DispatchDomainEvents(eventData.Context);
    //    return await base.SavingChangesAsync(eventData, result, cancellationToken);
    //}

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents(eventData.Context);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    //public async Task DispatchDomainEvents(DbContext? context)
    //{
    //    if (context == null) return;

    //    var aggregates = context.ChangeTracker
    //        .Entries<IAggregate>()
    //        .Where(a => a.Entity.DomainEvents.Any())
    //        .Select(a => a.Entity);

    //    var domainEvents = aggregates
    //        .SelectMany(a => a.DomainEvents)
    //        .ToList();

    //    aggregates.ToList().ForEach(a => a.ClearDomainEvents());

    //    foreach (var domainEvent in domainEvents)
    //        await mediator.Publish(domainEvent);
    //}

    public async Task DispatchDomainEvents(DbContext? context)
    {
        if (context == null) return;

        var aggregatesTemp = context.ChangeTracker
           .Entries<IAggregate>()
           .Select(a => a.Entity);

        var aggregates = context.ChangeTracker
            .Entries<IAggregate>()
            .Where(a => a.Entity.DomainEvents.Any())
            .Select(a => a.Entity);

        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ToList().ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            var domainEventNotification = 
                (INotification)Activator.CreateInstance(
                                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()),
                                domainEvent
                );

            await mediator.Publish(domainEventNotification);
        }
    }
}
