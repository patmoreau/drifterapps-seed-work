using System.Collections.Immutable;
using MediatR;

namespace DrifterApps.Seeds.Domain;

/// <summary>
///     Interface indicating that the class is a domain event publisher
/// </summary>
public interface IDomainPublisher
{
    /// <summary>
    ///     List of domain events
    /// </summary>
    public IImmutableList<INotification> DomainEvents { get; }

    /// <summary>
    ///     Add domain event to publisher
    /// </summary>
    /// <param name="eventItem"><see cref="INotification" /> event</param>
    public void AddDomainEvent(INotification eventItem);

    /// <summary>
    ///     Remove a domain event from publisher
    /// </summary>
    /// <param name="eventItem"><see cref="INotification" /> event</param>
    public void RemoveDomainEvent(INotification eventItem);

    /// <summary>
    ///     Clear all domain events from publisher
    /// </summary>
    public void ClearDomainEvents();
}
