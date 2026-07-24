using Bankyer.Domain.ValueObjects;
using Bankyer.Shared;
using Bankyer.Shared.Events;
using Bankyer.Shared.Exceptions;

namespace Bankyer.Domain.Aggregates;

public class Account : AggregateRoot
{
    public override string Name => "Account";
    
    public AccountStatus Status { get; private set; }
    public string Currency { get; private set; } = null!;
    public Money Balance { get; private set; } = null!;

    private Account() { }

    public static Account Create(Guid id, string currency, string userId)
    {
        var account = new Account();
        account.RaiseEvent(new AccountOpenedEvent(id, currency, userId));
        return account;
    }

    public static Account LoadFromHistory(IEnumerable<IDomainEvent> history)
    {
        var account = new Account();
        foreach (var @event in history)
        {
            account.Apply(@event);
        }
        return account;
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

        if (!string.Equals(amount.Currency, Currency, StringComparison.Ordinal))
        {
            return (false, "Deposit currency must match the account currency");
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

        RaiseEvent(new MoneyDepositedEvent(Id, new Money(amount.Amount, amount.Currency)));
    }

    public DomainValidation CanWithdraw(Money amount)
    {
        return new DomainValidation()
            .AddIf(Status != AccountStatus.Active, "account.not.active", "Account is not active")
            .AddIf(amount.Amount <= 0, "withdraw.amount.not.positive", "Withdraw amount must be positive")
            .AddIf(!string.Equals(amount.Currency, Currency, StringComparison.Ordinal), "withdraw.currency.invalid", "Withdrawal currency must match the account currency")
            .AddIf(Balance.Amount < amount.Amount, "insufficient.funds", "Insufficient funds");
    }

    public void Withdraw(Money amount)
    {
        CanWithdraw(amount).ThrowIfInvalid();
        RaiseEvent(new MoneyWithdrawnEvent(Id, amount));
    }

    protected void RaiseEvent(IDomainEvent @event)
    {
        Apply(@event);
        _domainEvents.Add(@event);
    }

    private void Apply(IDomainEvent @event)
    {
        Version++;
        switch (@event)
        {
            case AccountOpenedEvent e:
                Id = e.Id;
                Status = AccountStatus.Active;
                Currency = e.Currency;
                Balance = new Money(0, e.Currency);
                break;

            case MoneyDepositedEvent e:
                Balance = new Money(Balance.Amount + e.Money.Amount, Balance.Currency);
                break;

            case MoneyWithdrawnEvent e:
                Balance = new Money(Balance.Amount - e.Money.Amount, Balance.Currency);
                break;
        }
    }
}
