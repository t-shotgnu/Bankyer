using Bankyer.Domain;
using Bankyer.Shared.Events;

namespace Bankyer.Application.Handlers;

public class MoneyWithdrawnEventHandler : IEventHandler<MoneyWithdrawnEvent>
{
    public Task HandleAsync(MoneyWithdrawnEvent @event)
    {
        Console.WriteLine($"[EVENT HANDLED] Withdrawal recorded for account {@event.Id}: {@event.Amount} {@event.Currency}");
        return Task.CompletedTask;
    }
}