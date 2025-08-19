using AiTesting.Application.Users.Dto.Auth;
using AiTesting.Application.Users.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AiTesting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var result = await _authService.Register(dto);

        return result.IsSuccess ? 
               Ok(result.Value) : 
               BadRequest(result.Error);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);

        return result.IsSuccess ? 
               Ok(result.Value) :
               Unauthorized(new { message = result.Error });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.Refresh(dto);
        return result.IsSuccess ? 
               Ok(result.Value) : 
               Unauthorized(new { message = result.Error });
    }
}
