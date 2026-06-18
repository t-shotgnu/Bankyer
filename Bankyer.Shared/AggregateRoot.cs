namespace Bankyer.Shared;

public abstract class AggregateRoot
{
    public abstract string Name { get; }
    
    public Guid Id { get; protected set; }
    public int Version { get; protected set; } = 0;

    protected readonly List<Events.IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<Events.IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearChanges()
    {
        _domainEvents.Clear();
    }
}