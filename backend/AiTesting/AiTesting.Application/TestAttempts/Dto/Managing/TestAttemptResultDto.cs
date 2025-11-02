using AiTesting.Application.Tests.Dto.Managing;

namespace AiTesting.Application.TestAttempts.Dto.Managing;

public record TestAttemptResultDto
(
    string TestTitle,
    string DisplayUsername,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    List<QuestionDto> Questions,
    List<AttemptAnswerDto> Answers,
    double Score
);
