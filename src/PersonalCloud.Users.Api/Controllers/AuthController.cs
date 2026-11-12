using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using PersonalCloud.Users.Api.DTOs.Post;
using PersonalCloud.Users.Api.Services.Intrerfaces;
using PersonalCloud.Common.Security;

namespace PersonalCloud.Users.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserRequest request)
    {
        var result = await _userService.RegisterUser(
            request.Username,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName
        );

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
        [FromBody] LoginUserRequest request)
    {
            var user = await _userService.LoginUser(
                request.Username,
                request.Password
            );

                if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user)

            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return Ok(new { message = "Login successful" });
    }

    [Authorize]
    [HttpPost("logout")]

    public async Task<IActionResult> LogoutUser()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return Ok(new { message = "Logout successful" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.GetUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        return Ok(new { userId });
    }


}


