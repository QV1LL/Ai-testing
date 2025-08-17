using AiTesting.Application.Users.Command;
using AiTesting.Application.Users.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace AiTesting.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RegisterUserHandler _registerHandler;
    private readonly GetUserHandler _getUserHandler;

    public UsersController(RegisterUserHandler registerHandler,
                           GetUserHandler getUserHandler)
    {
        _registerHandler = registerHandler;
        _getUserHandler = getUserHandler;

    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var userResult = await _getUserHandler.Handle(new GetUserCommand(id));
        if (userResult.IsFailure) return NotFound();
        return Ok(userResult.Value);
    }



    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command)
    {
        var result = await _registerHandler.Handle(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(
            nameof(GetUserById),         
            new { id = result.Value.Id },  
            result.Value                  
        );
    }
}
