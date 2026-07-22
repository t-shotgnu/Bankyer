A value object representing a monetary amount in a specific [[currency]].

Money is the main and required part of [[transaction]].  Money without the context of [[transaction]] has no particular meaning.
## Invariants

- Money always has a [[currency]] and a value
- Money amount must be a non-negative value
- Any arithmetic operations require both operands to have the same [[currency]]