using AiTesting.Application.Users.Dto.Profile;
using AiTesting.Application.Users.Services.Profile;
using AiTesting.Domain.Common;
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
        var userIdResult = GetUserId();
        if (userIdResult.IsFailure)
            return Unauthorized(new { message = userIdResult.Error });

        var result = await _userProfileService.GetProfile(userIdResult.Value);

        if (result.IsSuccess)
            return Ok(result.Value);

        return NotFound(new { message = result.Error });
    }


    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto, IFormFile? avatarImage)
    {
        var userIdResult = GetUserId();
        if (userIdResult.IsFailure)
            return Unauthorized(new { message = userIdResult.Error });

        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";

        var result = await _userProfileService.UpdateProfile(userIdResult.Value, dto, avatarImage, baseUrl);

        return result.IsSuccess ?
               NoContent() :
               BadRequest(new { message = result.Error });        
    }

    [Authorize]
    [HttpDelete("profile")]
    public async Task<IActionResult> DeleteUser()
    {
        var userIdResult = GetUserId();
        if (userIdResult.IsFailure)
            return Unauthorized(new { message = userIdResult.Error });

        var result = await _userProfileService.DeleteProfile(userIdResult.Value);

        return result.IsSuccess ?
               NoContent() :
               BadRequest(new { message = result.Error });
    }

    private Result<Guid> GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Result<Guid>.Failure("Invalid token");

        return Result<Guid>.Success(Guid.Parse(userIdClaim.Value));
    }
}
