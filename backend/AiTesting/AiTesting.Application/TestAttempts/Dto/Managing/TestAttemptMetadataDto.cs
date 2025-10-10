namespace AiTesting.Application.TestAttempts.Dto.Managing;

public record TestAttemptMetadataDto
(
    Guid TestId,
    string? GuestName
);
