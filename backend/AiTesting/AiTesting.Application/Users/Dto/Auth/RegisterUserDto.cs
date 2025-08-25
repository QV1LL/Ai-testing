namespace AiTesting.Application.Users.Dto.Auth;

public record RegisterUserDto(
    string Name, 
    string Email, 
    string Password
);
