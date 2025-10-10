using AiTesting.Application.TestAttempts.Dto.Managing;
using AiTesting.Domain.Common;

namespace AiTesting.Application.TestAttempts.Services.Managing;

public interface ITestAttemptManageService
{
    Task<Result<TestAttemptMetadataDto>> GetById(Guid id);
    Task<Result<AddTestAttemptResultDto>> AddTestAttempt(AddTestAttemptDto dto);
    Task<Result<TestAttemptResultDto>> FinishTestAttempt(FinishTestAttemptDto dto);
}
