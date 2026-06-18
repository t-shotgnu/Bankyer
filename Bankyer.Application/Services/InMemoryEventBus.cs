using Bankyer.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Bankyer.Application.Services;

public class InMemoryEventBus(IServiceProvider serviceProvider, ILogger<InMemoryEventBus> logger) : IEventBus
{
    public async Task PublishAsync(IDomainEvent @event)
    {
        var eventType = @event.GetType();
        var eventName = eventType.Name;
        logger.LogInformation("Publishing integration event: {EventName}", eventName);

        using var scope = serviceProvider.CreateScope();

        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
        var handlers = scope.ServiceProvider.GetServices(handlerType).Cast<object>().ToList();

        if (handlers.Count == 0)
        {
            logger.LogWarning("No handlers found for event: {EventName}", eventName);
            return;
        }

        foreach (var handler in handlers)
        {
            try
            {
                logger.LogDebug("Executing handler {HandlerName} for event {EventName}", handler.GetType().Name, eventName);
                var handleMethod = handlerType.GetMethod("HandleAsync");
                if (handleMethod == null)
                {
                    continue;
                }

                var task = (Task?)handleMethod.Invoke(handler, [@event]);
                if (task != null)
                {
                    await task;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing handler {HandlerName} for event {EventName}", handler.GetType().Name, eventName);
            }
        }
    }
}