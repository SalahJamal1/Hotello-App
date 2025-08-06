using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApplication.Contracts;
using MyApplication.Data;
using MyApplication.Exceptions;
using MyApplication.Models.Users;

namespace MyApplication;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IAuthManager _authManager;


    public AuthController(IAuthManager authManager)
    {
        _authManager = authManager;
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiUser>> GetMe()
    {
        var user = await _authManager.GetUser();
        if (user == null) return Unauthorized();
        return Ok(user);
    }

    [HttpGet("logout")]
    [Authorize]
    public async Task<ActionResult> Logout()
    {
        await _authManager.Logout();
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthLogin user)
    {
        var response = await _authManager.Login(user);
        if (response == null) throw new AppErrorException("invalid credentials");
        return Ok(response);
    }

    [HttpPost("signup")]
    public async Task<ActionResult> SignUp([FromBody] ApiUserDto user)
    {
        var errors = await _authManager.Register(user);
        if (errors.Any())
        {
            foreach (var error in errors)
                ModelState.AddModelError(error.Code, error.Description);
            return BadRequest(ModelState);
        }

        return Ok();
    }
}