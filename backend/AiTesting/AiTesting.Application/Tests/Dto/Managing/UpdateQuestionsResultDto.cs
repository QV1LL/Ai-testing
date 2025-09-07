namespace AiTesting.Application.Tests.Dto.Managing;

public record UpdateQuestionsResultDto
(
    Dictionary<Guid, Guid> QuestionTempToRegularIds,
    Dictionary<Guid, Guid> OptionTempToRegularIds
);
