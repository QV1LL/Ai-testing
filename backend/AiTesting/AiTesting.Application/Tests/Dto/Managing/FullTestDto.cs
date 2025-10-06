namespace AiTesting.Application.Tests.Dto.Managing;

public record FullTestDto(
    Guid Id,
    string Title,
    string? Description,
    string? CoverImageUrl,
    bool IsPublic,
    int? TimeLimitMinutes,
    string JoinId,
    List<QuestionDto> Questions,
    List<TestAttemptDto> TestAttempts,
    int AttemptsCount,
    double AverageScore
);
