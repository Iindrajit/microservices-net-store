//using MediatR;

//namespace Ordering.Domain.Abstractions;
//public interface IDomainEvent : INotification
//{
//    Guid EventId => Guid.NewGuid();
//    public DateTime OccurredOn => DateTime.Now;
//    public string EventType => GetType().AssemblyQualifiedName;
//}


namespace Ordering.Domain.Abstractions;

/// <summary>
/// IDomainEvent - Represents a domain event in the context of Domain-Driven Design (DDD).
/// </summary>
public interface IDomainEvent
{
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName!;
}