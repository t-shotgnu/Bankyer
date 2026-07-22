Is a record of financial event which influences the state of one or more [[Account|accounts]].

Transaction consists:
- Operation type
- Date
- [[Money]]
- Source [[Account]]
- Destination [[Account]]
- Status

Operation types include:
- Transfer
- [[Cash Deposit]]
- Withdrawal
- Payment
- Fee
- Interest

## Invariants

- All transactions no matter their type is, if they influence more than one [[account]], must operate on the same currency