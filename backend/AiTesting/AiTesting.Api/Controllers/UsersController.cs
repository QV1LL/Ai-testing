using AiTesting.Application.Users.Dto.Profile;
using AiTesting.Application.Users.Services.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTesting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UsersController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { message = "Invalid token" });

        var userId = Guid.Parse(userIdClaim.Value);

        var result = await _userProfileService.GetProfile(userId);

        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(new { message = result.Error });
    }


    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { message = "Invalid token" });

        var userId = Guid.Parse(userIdClaim.Value);
        var result = await _userProfileService.UpdateProfile(userId, dto);

        return result.IsSuccess ?
               NoContent() :
               BadRequest(new { message = result.Error });        
    }

    [Authorize]
    [HttpDelete("profile")]
    public async Task<IActionResult> DeleteUser()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { message = "Invalid token" });

        var userId = Guid.Parse(userIdClaim.Value);
        var result = await _userProfileService.DeleteProfile(userId);

        return result.IsSuccess ?
               NoContent() :
               BadRequest(new { message = result.Error });
    }
}
