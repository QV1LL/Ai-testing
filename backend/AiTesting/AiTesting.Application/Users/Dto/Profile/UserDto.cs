namespace AiTesting.Application.Users.Dto.Profile;

public record UserDto(
    string Name,
    string Email,
    string AvatarUrl,
    List<TestProfileDto> Tests,
    List<TestAttemptProfileDto> TestAttempts
);
