namespace Bankyer.Application.Queries;

public record GetAllAccountsResponse
{
    public required Guid Id { get; init; }
    public required string Status { get; init; }
    public required decimal Balance { get; init; }
    public required GetCurrencyResponse Currency { get; init; }
    
    public record GetCurrencyResponse
    {
        public required string Code { get; init; }
        public required string Name { get; init; }
    }
}

