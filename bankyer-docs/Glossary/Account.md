## Definition

An account represents a bank account owned by a customer and maintained by the bank.
It holds a monetary balance in exactly one currency and records all [[Transaction|transactions]] affecting its balance.

## Responsibilities

- Hold funds in a single currency in form of set of [[Transaction|transactions]]
- Accept credits and debits.
- Maintain a history of [[Transaction|transactions]].
- Expose current [[balance]].
- Enforce domain invariants.

## Invariants

- An account has exactly one [[customer|owner]].
- An account is denominated in exactly one [[currency]].
- The account **[[currency]] cannot be changed after creation**.
- Every [[transaction]] must use the same currency as the account.
- A closed account cannot accept new [[Transaction|transactions]].

## Lifecycle

1. Created
2. Active
3. Suspended (optional)
4. Closed
## Notes

An account is not the same as a banking product.
A [[customer]] may own multiple accounts (e.g. PLN, EUR, USD) under a single product.