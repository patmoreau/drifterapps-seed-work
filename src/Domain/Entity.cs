using MediatR;

namespace DrifterApps.Seeds.Domain;

/// <summary>
///
/// </summary>
public abstract record Entity
{
    private readonly List<INotification> _domainEvents = new();
    /// <summary>
    ///
    /// </summary>
    public virtual Guid Id { get; init; }

    /// <summary>
    ///
    /// </summary>
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    ///
    /// </summary>
    /// <param name="eventItem"></param>
    public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);

    /// <summary>
    ///
    /// </summary>
    /// <param name="eventItem"></param>
    public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);

    /// <summary>
    ///
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
