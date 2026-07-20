using Bankyer.Domain.ValueObjects;
using Bankyer.Application.Commands.CreateAccount;
using Bankyer.Application.Commands.DeleteAccount;
using Bankyer.Application.Commands.Deposit;
using Bankyer.Application.Commands.Withdraw;
using Bankyer.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Bankyer.Api.Controllers;

/// <summary>
/// Manage bank accounts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AccountsController(
    CreateAccountCommandHandler createAccountCommandHandler,
    DepositCommandHandler depositCommandHandler,
    WithdrawCommandHandler withdrawCommandHandler,
    DeleteAccountCommandHandler deleteAccountCommandHandler,
    GetAccountQueryHandler getAccountQueryHandler,
    GetAllAccountsQueryHandler getAllAccountsQueryHandler)
    : ControllerBase
{
    /// <summary>Retrieve all accounts for development and back-office tooling.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAccounts()
    {
        return Ok(await getAllAccountsQueryHandler.HandleAsync());
    }

    /// <summary>
    /// Retrieve account by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the account.</param>
    /// <returns>The account details.</returns>
    /// <response code="200">Returns the requested account.</response>
    /// <response code="404">If the account is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        var entity = await getAccountQueryHandler.Handle(new GetAccountQuery(id), CancellationToken.None);
        if (entity == null)
        {
            return NotFound();
        }

        return Ok(entity);
    }

    /// <summary>
    /// Create a new account.
    /// </summary>
    /// <param name="request">The initial amount and currency for the account.</param>
    /// <returns>The newly created account.</returns>
    /// <response code="201">Returns the newly created account.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var entity = await createAccountCommandHandler.Handle(new CreateAccountCommand(request.InitialAmount, request.Currency), CancellationToken.None);
        return CreatedAtAction(nameof(GetAccount), new { id = entity }, entity);
    }

    /// <summary>
    /// Deposit money into account.
    /// </summary>
    /// <param name="id">The account identifier.</param>
    /// <param name="request">The amount and currency to deposit.</param>
    /// <returns>The updated account details.</returns>
    /// <response code="200">Returns the updated account.</response>
    /// <response code="400">If the deposit is invalid.</response>
    /// <response code="404">If the account is not found.</response>
    [HttpPost("{id}/deposit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] MoneyRequest request)
    {
        try
        {
            var entity = await depositCommandHandler.Handle(new DepositCommand(id, request.Amount, request.Currency),
                CancellationToken.None);

            return Ok(entity);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Withdraw money from account.
    /// </summary>
    /// <param name="id">The account identifier.</param>
    /// <param name="request">The amount and currency to withdraw.</param>
    /// <returns>The updated account details.</returns>
    /// <response code="200">Returns the updated account.</response>
    /// <response code="400">If the withdrawal is invalid (e.g. insufficient funds).</response>
    /// <response code="404">If the account is not found.</response>
    [HttpPost("{id}/withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Withdraw(Guid id, [FromBody] MoneyRequest request)
    {
        var withdrawResult = await withdrawCommandHandler.Handle(new WithdrawCommand(id, request.Amount, request.Currency),
            CancellationToken.None);
        if (withdrawResult.Errors.Count > 0)
        {
            return BadRequest(withdrawResult.Errors);
        }

        return Ok(withdrawResult);
    }

    /// <summary>
    /// Delete account.
    /// </summary>
    /// <param name="id">The account identifier.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">If the account was successfully deleted.</response>
    /// <response code="404">If the account is not found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await deleteAccountCommandHandler.Handle(new DeleteAccountCommand(id), CancellationToken.None);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}

/// <summary>
/// Provide request payload to create a new account.
/// </summary>
/// <param name="InitialAmount">The initial deposit amount.</param>
/// <param name="Currency">The currency of the account.</param>
public record CreateAccountRequest(decimal InitialAmount, Currency Currency);

/// <summary>
/// Provide request payload to transfer money.
/// </summary>
/// <param name="Amount">The money amount.</param>
/// <param name="Currency">The money currency.</param>
public record MoneyRequest(decimal Amount, Currency Currency);

