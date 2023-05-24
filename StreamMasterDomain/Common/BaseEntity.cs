using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StreamMasterDomain.Common;

public abstract class BaseEntity
{
    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped]
    [JsonIgnore]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    [Key]
    public int Id { get; set; }

    public void AddDomainEvent(BaseEvent @domainEvent)
    {
        _domainEvents.Add(@domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void RemoveDomainEvent(BaseEvent @domainEvent)
    {
        _ = _domainEvents.Remove(@domainEvent);
    }
}
