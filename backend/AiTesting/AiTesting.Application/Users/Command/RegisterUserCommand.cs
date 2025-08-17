namespace AiTesting.Application.Users.Command;

public record RegisterUserCommand(string Name, string Email, string Password)
{
}
