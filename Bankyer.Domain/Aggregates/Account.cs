using Bankyer.Domain.ValueObjects;
using Bankyer.Shared;
using Bankyer.Shared.Events;

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

    public void Deposit(Money amount)
    {
        EnsureAccountIsActive();

        if (amount.Amount <= 0)
        {
            throw new InvalidOperationException("Deposit amount must be positive");
        }

        if (Balance != null && Balance.Currency != amount.Currency)
        {
            throw new InvalidOperationException("Currency mismatch");
        }

        RaiseEvent(new MoneyDepositedEvent(Id, amount.Amount, amount.Currency));    
    }

    public void Withdraw(Money amount)
    {
        EnsureAccountIsActive();

        if (Balance.Currency != amount.Currency)
        {
            throw new InvalidOperationException("Currency mismatch");
        }

        if (amount.Amount <= 0)
        {
            throw new InvalidOperationException("Withdraw amount must be positive");
        }

        if (Balance.Amount < amount.Amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }

        RaiseEvent(new MoneyWithdrawnEvent(Id, amount.Amount, amount.Currency));
    }

    protected void RaiseEvent(IDomainEvent @event)
    {
        Apply(@event);
        Version++;
        _domainEvents.Add(@event);
    }

    public void LoadFromHistory(IEnumerable<IDomainEvent> history)
    {
        foreach (var @event in history)
        {
            Apply(@event);
            Version++;
        }
    }

    private void Apply(IDomainEvent @event)
    {
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