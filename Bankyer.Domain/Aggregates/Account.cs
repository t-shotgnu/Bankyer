using Bankyer.Domain.ValueObjects;
using Bankyer.Shared;
using Bankyer.Shared.Events;
using Bankyer.Shared.Exceptions;

namespace Bankyer.Domain.Aggregates;

public class Account() : AggregateRoot
{
    public override string Name => "Account";
    
    public Money Balance { get; private set; } = new(0, Currency.Eur);
    public AccountStatus Status { get; private set; } = AccountStatus.InFormation;

    public Account(Guid id, Currency currency) : this()
    {
        RaiseEvent(new AccountOpenedEvent(id, currency));
    }

    private void EnsureAccountIsActive()
    {
        if (Status != AccountStatus.Active)
        {
            throw new InvalidOperationException("Account is not active");
        }
    }
    
    public (bool CanDeposit, string? Reason) CanDeposit(Money amount)
    {
        if (Status != AccountStatus.Active)
        {
            return (false, "Account is not active");
        }

        if (amount.Amount <= 0)
        {
            return (false, "Deposit amount must be positive");
        }

        if (Balance.Currency != amount.Currency)
        {
            return (false, "Currency mismatch");
        }

        return (true, null);
    }

    public void Deposit(Money amount)
    {
        EnsureAccountIsActive();
        
        var (canDeposit, reason) = CanDeposit(amount);
        if (!canDeposit)
        {
            throw new DomainViolationException(new List<DomainRuleViolation>
            {
                new DomainRuleViolation("deposit.validation.failed", reason ?? "Unknown reason"),
            });
        }

        RaiseEvent(new MoneyDepositedEvent(Id, amount.Amount, amount.Currency));    
    }

    public DomainValidation CanWithdraw(Money amount)
    {
        return new DomainValidation()
            .AddIf(Status != AccountStatus.Active, "account.not.active", "Account is not active")
            .AddIf(amount.Amount <= 0, "withdraw.amount.not.positive", "Withdraw amount must be positive")
            .AddIf(Balance.Currency != amount.Currency, "currency.mismatch", "Currency mismatch")
            .AddIf(Balance.Amount < amount.Amount, "insufficient.funds", "Insufficient funds");
    }

    public void Withdraw(Money amount)
    {
        CanWithdraw(amount).ThrowIfInvalid();

        RaiseEvent(new MoneyWithdrawnEvent(Id, amount.Amount, amount.Currency));
    }

    protected void RaiseEvent(IDomainEvent @event)
    {
        Apply(@event);
        _domainEvents.Add(@event);
    }

    public void LoadFromHistory(IEnumerable<IDomainEvent> history)
    {
        foreach (var @event in history)
        {
            Apply(@event);
        }
    }

    private void Apply(IDomainEvent @event)
    {
        Version++;
        switch (@event)
        {
            case AccountOpenedEvent e:
                Id = e.Id;
                Status = AccountStatus.Active;
                Balance = new Money(0, e.Currency);
                break;

            case MoneyDepositedEvent e:
                Balance = new Money(Balance.Amount + e.Amount, Balance.Currency);
                break;

            case MoneyWithdrawnEvent e:
                Balance = new Money(Balance.Amount - e.Amount, Balance.Currency);
                break;
        }
    }
}