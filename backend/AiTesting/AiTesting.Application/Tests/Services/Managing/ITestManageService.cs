using AiTesting.Application.Tests.Dto.Managing;
using AiTesting.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Tests.Services.Managing;

public interface ITestManageService
{
    Task<Result<UserTestsResultDto>> GetUserTests(Guid ownerId);
    Task<Result<FullTestDto>> Get(Guid id);
    Task<Result<CreateTestResultDto>> Create(CreateTestDto dto, IFormFile? coverImage, Guid ownerId);
    Task<Result> UpdateMetadata(UpdateTestMetadataDto dto, Guid ownerId);
    Task<Result<UpdateQuestionsResultDto>> UpdateQuestions(UpdateQuestionsDto dto, Guid ownerId);
    Task<Result> UpdateQuestionImage(UpdateQuestionImageDto dto, Guid ownerId);
    Task<Result> UpdateOptionImage(UpdateOptionImageDto dto, Guid ownerId);
    Task<Result> Delete(Guid testId, Guid ownerId);
}