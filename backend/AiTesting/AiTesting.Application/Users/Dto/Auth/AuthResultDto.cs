namespace AiTesting.Application.Users.Dto.Auth;

public record AuthResultDto(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string DisplayName,
    string Email
);
