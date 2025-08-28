namespace AiTesting.Application.Users.Dto.Profile;

public record TestProfileDto(
    Guid Id,
    string Title,
    DateTimeOffset CreatedAt
);
