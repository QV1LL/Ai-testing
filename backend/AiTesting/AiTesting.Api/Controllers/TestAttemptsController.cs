using AiTesting.Application.TestAttempts.Dto.Managing;
using AiTesting.Application.TestAttempts.Services.Managing;
using Microsoft.AspNetCore.Mvc;

namespace AiTesting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestAttemptsController : ControllerBase
{
    private readonly ITestAttemptManageService _testAttemptManageService;

    public TestAttemptsController(ITestAttemptManageService testAttemptManageService)
    {
        _testAttemptManageService = testAttemptManageService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(AddTestAttemptDto dto)
    {
        var result = await _testAttemptManageService.AddTestAttempt(dto);

        return result.IsSuccess ? 
               Ok(result) : 
               BadRequest(result.Error);
    }
}
