using AiTesting.Application.TestAttempts.Dto.Managing;
using AiTesting.Application.TestAttempts.Services.Managing;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

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

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _testAttemptManageService.GetById(id);

        return result.IsSuccess ?
               Ok(result.Value) :
               BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddTestAttemptDto dto)
    {
        var result = await _testAttemptManageService.AddTestAttempt(dto);

        return result.IsSuccess ? 
               Ok(result.Value) : 
               BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> Finish([FromBody] FinishTestAttemptDto dto)
    {
        var result = await _testAttemptManageService.FinishTestAttempt(dto);

        return result.IsSuccess ?
               Ok(result.Value) :
               BadRequest(result.Error);
    }
}
