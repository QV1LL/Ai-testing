namespace AiTesting.Application.Tests.Dto.Managing;

public record UpdateQuestionsDto(
    Guid TestId,
    List<UpdateQuestionDto> QuestionsToAdd,
    List<Guid> QuestionsToDeleteIds,
    List<UpdateQuestionDto> QuestionsToUpdate
);
