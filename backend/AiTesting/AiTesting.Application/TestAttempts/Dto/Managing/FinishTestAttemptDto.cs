namespace AiTesting.Application.TestAttempts.Dto.Managing;

public record FinishTestAttemptDto
(
    Guid AttemptId,
    List<AttemptAnswerDto> Answers
);
