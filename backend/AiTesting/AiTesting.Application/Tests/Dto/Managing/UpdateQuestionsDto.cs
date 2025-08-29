namespace AiTesting.Application.Tests.Dto.Managing;

public record UpdateQuestionsDto(
    Guid TestId,
    List<UpdateQuestionDto> QuestionsToAdd,
    List<UpdateQuestionDto> QuestionsToUpdate,
    List<Guid> QuestionsToDeleteIds
);
