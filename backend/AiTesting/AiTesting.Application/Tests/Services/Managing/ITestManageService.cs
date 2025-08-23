using AiTesting.Application.Tests.Dto.Managing;
using AiTesting.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Tests.Services.Managing;

public interface ITestManageService
{
    Task<Result<UserTestsResultDto>> GetUserTests(Guid ownerId);
    Task<Result<CreateTestResultDto>> Create(CreateTestDto dto, IFormFile? coverImage, Guid ownerId, string apiUrl);
}
