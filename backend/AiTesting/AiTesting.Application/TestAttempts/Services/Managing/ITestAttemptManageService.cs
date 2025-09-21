using AiTesting.Application.TestAttempts.Dto.Managing;
using AiTesting.Domain.Common;

namespace AiTesting.Application.TestAttempts.Services.Managing;

public interface ITestAttemptManageService
{
    Task<Result> AddTestAttempt(AddTestAttemptDto dto);
}
