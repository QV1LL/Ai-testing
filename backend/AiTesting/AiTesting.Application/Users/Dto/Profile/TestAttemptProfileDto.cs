namespace AiTesting.Application.Users.Dto.Profile;

public record TestAttemptProfileDto(
    Guid Id,
    string Title,
    DateTimeOffset StartedAt,
    double Score
)
{
}
