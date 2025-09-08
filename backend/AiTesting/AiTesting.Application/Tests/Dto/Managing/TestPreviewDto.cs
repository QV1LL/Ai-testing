namespace AiTesting.Application.Tests.Dto.Managing;

public record TestPreviewDto
(
    Guid Id,
    string Title,
    string? Description,
    string? CoverImageUrl,
    int? TimeLimitMinutes
);
