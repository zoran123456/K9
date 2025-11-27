using System.ComponentModel.DataAnnotations.Schema;

namespace K9.SharedKernel.Domain;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity(Guid id)
    {
        Id = id;
    }

    protected Entity() { }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}