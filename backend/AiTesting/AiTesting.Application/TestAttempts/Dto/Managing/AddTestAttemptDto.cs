namespace AiTesting.Application.TestAttempts.Dto.Managing;

public record AddTestAttemptDto
(
    Guid TestId,
    Guid? UserId,
    string? GuestName,
    DateTimeOffset StartedAt
);
