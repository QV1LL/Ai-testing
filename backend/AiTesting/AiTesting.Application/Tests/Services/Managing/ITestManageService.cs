using AiTesting.Application.Tests.Dto.Managing;
using AiTesting.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Tests.Services.Managing;

public interface ITestManageService
{
    Task<Result<UserTestsResultDto>> GetUserTests(Guid ownerId);
    Task<Result<FullTestDto>> Get(Guid id);
    Task<Result<CreateTestResultDto>> Create(CreateTestDto dto, IFormFile? coverImage, Guid ownerId, string apiUrl);
    Task<Result> Delete(Guid testId, Guid ownerId, string apiUrl);
}
