using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bankyer.Api.Controllers;

/// <summary>
/// Provides information about the authenticated user.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager) : ControllerBase
{
    /// <summary>Creates a new user account.</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(string.Join(" ", result.Errors.Select(error => error.Description)));
        }

        return Ok();
    }

    /// <summary>Signs in with the application cookie.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await signInManager.PasswordSignInAsync(
            request.Email,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        return Ok();
    }

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

/// <summary>Credentials used to register a user.</summary>
public record RegisterRequest(string Email, string Password);

/// <summary>Credentials used to sign in.</summary>
public record LoginRequest(string Email, string Password);

/// <summary>Public details for the signed-in user.</summary>
public record CurrentUserResponse(string Id, string? UserName, string? Email);
