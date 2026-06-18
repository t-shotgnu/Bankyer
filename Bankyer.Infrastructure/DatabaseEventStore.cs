using System.Text.Json;
using Bankyer.Infrastructure.Database;
using Bankyer.Infrastructure.Database.Entities;
using Bankyer.Shared;
using Bankyer.Shared.Events;
using Microsoft.EntityFrameworkCore;

namespace Bankyer.Infrastructure;

public class DatabaseEventStore(AppDbContext dbContext, IEventBus eventBus, IEventTypeResolver eventTypeResolver) : IEventStore
{
    public async Task<IReadOnlyList<IDomainEvent>> GetEventsAsync(Guid aggregateId)
    {
        var rows = await dbContext.EventStore
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .ToListAsync();

        var events = new List<IDomainEvent>();

        foreach (var row in rows)
        {
            var type = eventTypeResolver.Resolve(row.EventName);
            if (type == null)
            {
                continue;
            }

            if (JsonSerializer.Deserialize(row.Payload, type) is IDomainEvent @event)
            {
                events.Add(@event);
            }
        }

        return events;
    }

    public async Task<IReadOnlyList<EventRecord>> GetEventRecordsAsync(Guid aggregateId)
    {
        var rows = await dbContext.EventStore
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.Version)
            .ToListAsync();

        var records = new List<EventRecord>();

        foreach (var row in rows)
        {
            var type = eventTypeResolver.Resolve(row.EventName);
            if (type == null)
            {
                continue;
            }

            if (JsonSerializer.Deserialize(row.Payload, type) is IDomainEvent @event)
            {
                records.Add(new EventRecord(@event, row.CreatedAt));
            }
        }

        return records;
    }
    
    public async Task SaveEventsAsync(Guid aggregateId, string aggregateType, IReadOnlyCollection<IDomainEvent> events, long expectedVersion)
    {
        if (events.Count == 0)
        {
            return;
        }

        long currentVersion = expectedVersion - events.Count;

        foreach (IDomainEvent @event in events)
        {
            currentVersion++;

            var eventRow = new EventStoreItem
            {
                AggregateId = aggregateId,
                AggregateType = aggregateType,
                EventName = @event.GetType().Name,
                Version = (int)currentVersion,
                Payload = JsonSerializer.Serialize(@event, @event.GetType()),
                CreatedAt = DateTime.UtcNow,
            };

            await dbContext.EventStore.AddAsync(eventRow);
        }

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            dbContext.ChangeTracker.Clear();
            throw;
        }

        foreach (var @event in events)
        {
            await eventBus.PublishAsync(@event);
        }
    }
}