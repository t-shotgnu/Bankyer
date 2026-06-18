using System.Text.Json.Serialization;

namespace Bankyer.Application.Queries;

public record GetAccountResponse
{
    public required Guid Id { get; init; }
    public required decimal Balance { get; init; }
    public required string Status { get; init; }
    public required List<Transaction> Transactions { get; init; }

    public record Transaction
    {
        public required decimal Amount { get; init; }
        public required DateTime Date { get; init; }
        public required TransactionType Type { get; init; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
    }
}