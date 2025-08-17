using AiTesting.Application.Users.Command;
using AiTesting.Application.Users.Services;
using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using AiTesting.Domain.Services.User;

namespace AiTesting.Application.Users.Handlers;

public class RegisterUserHandler
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUserService userService, IPasswordHasher passwordHasher)
    {
        _userService = userService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<User>> Handle(RegisterUserCommand command)
    {
        var hashedPassword = _passwordHasher.HashPassword(command.Password);
        var userResult = User.CreateDefault(command.Name, command.Email, hashedPassword);

        if (!userResult.IsSuccess)
            return Result<User>.Failure(userResult.Error);

        var user = userResult.Value;
        await _userService.Add(user);

        return Result<User>.Success(user);
    }
}
