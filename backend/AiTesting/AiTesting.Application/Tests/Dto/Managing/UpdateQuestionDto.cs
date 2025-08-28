using AiTesting.Domain.Enums;

namespace AiTesting.Application.Tests.Dto.Managing;

public record UpdateQuestionDto(
    Guid Id,
    string Text,
    int Order,
    string? CorrectTextAnswer,
    QuestionType Type,
    List<UpdateOptionDto> Options,
    List<UpdateOptionDto> CorrectAnswers
);
