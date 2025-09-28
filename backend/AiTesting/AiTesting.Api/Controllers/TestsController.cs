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

        var result = await _testManageService.Create(dto, coverImage, ownerIdResult.Value);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPut("metadata")]
    public async Task<IActionResult> UpdateMetadata([FromForm] UpdateTestMetadataDto dto)
    {
        var ownerIdResult = GetUserId();
        if (ownerIdResult.IsFailure)
            return Unauthorized(new { message = ownerIdResult.Error });

        var result = await _testManageService.UpdateMetadata(dto, ownerIdResult.Value);

        return result.IsSuccess ?
               NoContent() :
               BadRequest(new { message = result.Error });
    }

    [Authorize]
    [HttpPut("questions")]
    public async Task<IActionResult> UpdateQuestions([FromBody] UpdateQuestionsDto dto)
    {
        var ownerIdResult = GetUserId();
        if (ownerIdResult.IsFailure)
            return Unauthorized(new { message = ownerIdResult.Error });

        var result = await _testManageService.UpdateQuestions(dto, ownerIdResult.Value);

        return result.IsSuccess ?
               Ok(result.Value) :
               BadRequest(new { message = result.Error });
    }

    [Authorize]
    [HttpPost("question/image")]
    public async Task<IActionResult> UpdateQuestionImage([FromForm] UpdateQuestionImageDto dto)
    {
        var ownerIdResult = GetUserId();
        if (ownerIdResult.IsFailure)
            return Unauthorized(new { message = ownerIdResult.Error });

        var result = await _testManageService.UpdateQuestionImage(dto, ownerIdResult.Value);

        return result.IsSuccess ?
               Ok() :
               BadRequest(new { message = result.Error });
    }

    [Authorize]
    [HttpPost("question/option/image")]
    public async Task<IActionResult> UpdateOptionImage([FromForm] UpdateOptionImageDto dto)
    {
        var ownerIdResult = GetUserId();
        if (ownerIdResult.IsFailure)
            return Unauthorized(new { message = ownerIdResult.Error });

        var result = await _testManageService.UpdateOptionImage(dto, ownerIdResult.Value);

        return result.IsSuccess ?
               Ok() :
               BadRequest(new { message = result.Error });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ownerIdResult = GetUserId();
        if (ownerIdResult.IsFailure)
            return Unauthorized(new { message = ownerIdResult.Error });

        var result = await _testManageService.Delete(id, ownerIdResult.Value);

        return result.IsSuccess ?
                NoContent() :
                BadRequest(new { message = result.Error });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _testManageService.GetFull(id);

        if (result.IsFailure) return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("preview/{id}")]
    public async Task<IActionResult> GetTestPreview(Guid id)
    {
        var result = await _testManageService.GetPreview(id);

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
