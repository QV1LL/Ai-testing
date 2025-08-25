namespace AiTesting.Application.Tests.Dto.Managing;

public record CreateTestDto
(
    string Title,
    string? Description,
    bool IsPublic,
    int? TimeLimitMinutes
);
