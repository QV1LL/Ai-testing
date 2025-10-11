namespace AiTesting.Application.TestAttempts.Dto.Managing;

public record AddTestAttemptDto
(
    string TestJoinId,
    Guid? UserId,
    string? GuestName,
    DateTimeOffset StartedAt
);
