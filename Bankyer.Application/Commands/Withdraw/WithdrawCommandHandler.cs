using Bankyer.Domain.Aggregates;
using Bankyer.Domain.ValueObjects;
using Bankyer.Shared;

namespace Bankyer.Application.Commands.Withdraw;

public record WithdrawCommandResult
{
    public IReadOnlyList<string> Errors { get; init; } = [];
    public bool IsSuccess => Errors.Count == 0;
}

public class WithdrawCommandHandler(IEventStore eventStore)
{
    public async Task<WithdrawCommandResult> Handle(WithdrawCommand request, CancellationToken cancellationToken = default)
    {
        var history = await eventStore.GetEventsAsync(request.Id);
        if (!history.Any())
        {
            return new WithdrawCommandResult();
        }

        var account = new Account();
        account.LoadFromHistory(history);

        var amountToWithdraw = new Money(request.Amount, request.Currency);
        var canWithdrawValidation = account.CanWithdraw(amountToWithdraw);
        
        if (canWithdrawValidation.HasViolations)
        {
            return new WithdrawCommandResult
            {
                Errors = canWithdrawValidation.Violations.Select(v => v.Message).ToList(),
            };
        }

        account.Withdraw(amountToWithdraw);

        await eventStore.SaveEventsAsync(
            account.Id,
            "Account",
            account.DomainEvents,
            account.Version
        );

        account.ClearChanges();

        return new WithdrawCommandResult();
    }
}
