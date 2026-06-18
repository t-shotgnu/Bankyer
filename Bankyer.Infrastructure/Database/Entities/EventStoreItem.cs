using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Infrastructure.Database.Entities;

[Table("EventStore")]
[Index(nameof(AggregateId), nameof(Version), IsUnique = true)]
public class EventStoreItem
{
    public int Id { get; set; }
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; }
    public string EventName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Payload { get; set; }
    public int Version { get; set; }
}