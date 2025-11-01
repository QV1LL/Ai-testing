using AiTesting.Domain.Enums;

namespace AiTesting.Application.Tests.Dto.Managing;

public record QuestionDto(
    Guid Id,
    string Text,
    string? ImageUrl,
    string? CorrectTextAnswer,
    int Order,
    QuestionType Type,
    List<AnswerOptionDto> Options,
    List<AnswerOptionDto> CorrectAnswers
);
