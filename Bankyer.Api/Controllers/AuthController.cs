using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bankyer.Api.Controllers;

/// <summary>
/// Provides information about the authenticated user.
/// </summary>
/// <remarks>
/// Registration, login, token refresh, password reset, and logout are provided by
/// ASP.NET Core Identity at <c>/api/auth</c>.
/// </remarks>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(UserManager<IdentityUser> userManager) : ControllerBase
{
    /// <summary>Gets the currently authenticated user's public identity details.</summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserResponse(
            user.Id,
            user.UserName,
            User.FindFirstValue(ClaimTypes.Email) ?? user.Email));
    }
}

/// <summary>Public details for the signed-in user.</summary>
public record CurrentUserResponse(string Id, string? UserName, string? Email);
