namespace Bankyer.Application.Queries;

public class GetAllAccountsResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = null!;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = null!;
}
