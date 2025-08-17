using AiTesting.Application.Users.Command;
using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using AiTesting.Domain.Services.User;

namespace AiTesting.Application.Users.Handlers;

public class GetUserHandler
{
    private readonly IUserService _userService;

    public GetUserHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result<User>> Handle(GetUserCommand command)
    {
        if (command.Id == Guid.Empty)
            return Result<User>.Failure("User Id cannot be empty.");

        var userResult = await _userService.Get(command.Id);

        return userResult; 
               
    }
}
