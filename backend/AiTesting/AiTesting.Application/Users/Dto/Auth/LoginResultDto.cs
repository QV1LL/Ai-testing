namespace AiTesting.Application.Users.Dto.Auth;

public record LoginResultDto(
    string Token,
    Guid UserId,
    string DisplayName,
    string Email
)
{
}
