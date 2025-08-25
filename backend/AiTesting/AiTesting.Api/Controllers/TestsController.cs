using AiTesting.Application.Tests.Dto.Managing;
using AiTesting.Application.Tests.Services.Managing;
using AiTesting.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiTesting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController : ControllerBase
{
    private readonly ITestManageService _testManageService;

    public TestsController(ITestManageService testManageService)
    {
        _testManageService = testManageService;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CreateTestDto dto, IFormFile? coverImage)
    {
        var ownerIdResult = GetUserId();
        if (ownerIdResult.IsFailure)
            return Unauthorized(new { message = ownerIdResult.Error });

        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";

        var result = await _testManageService.Create(dto, coverImage, ownerIdResult.Value, baseUrl);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id )
    {
        var ownerIdResult = GetUserId();
        if (ownerIdResult.IsFailure)
            return Unauthorized(new { message = ownerIdResult.Error });

        var request = HttpContext.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";

        var result = await _testManageService.Delete(id, ownerIdResult.Value, baseUrl);

        return result.IsSuccess ?
                NoContent() :
                BadRequest(new { message = result.Error });
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _testManageService.Get(id);

        if (result.IsFailure) return NotFound(result.Error);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUserTests()
    {
        var ownerIdResult = GetUserId();
        if (ownerIdResult.IsFailure)
            return Unauthorized(new { message = ownerIdResult.Error });

        var result = await _testManageService.GetUserTests(ownerIdResult.Value);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    private Result<Guid> GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Result<Guid>.Failure("Invalid token");

        return Result<Guid>.Success(Guid.Parse(userIdClaim.Value));
    }
}
