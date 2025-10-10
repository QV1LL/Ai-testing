namespace AiTesting.Application.TestAttempts.Dto.Managing;

public record TestAttemptResultDto
(
    string TestTitle,
    string DisplayUsername,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    double Score
);
