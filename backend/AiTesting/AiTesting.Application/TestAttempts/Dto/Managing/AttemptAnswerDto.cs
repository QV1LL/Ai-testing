using AiTesting.Application.Tests.Dto.Managing;

namespace AiTesting.Application.TestAttempts.Dto.Managing;

public record AttemptAnswerDto
(
    Guid QuestionId,
    List<AnswerOptionDto> SelectedOptions,
    string? WrittenAnswer
);
