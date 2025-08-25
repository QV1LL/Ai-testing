namespace AiTesting.Application.Tests.Dto.Managing;

public record TestAttemptDto(
    Guid Id,
    string UserDisplayName,
    string UserAvatarUrl,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    double Score
);
