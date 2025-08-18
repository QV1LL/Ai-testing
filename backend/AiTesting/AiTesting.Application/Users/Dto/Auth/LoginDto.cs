namespace AiTesting.Application.Users.Dto.Auth;

public record LoginDto(
    string Email, 
    string Password
)
{
}
