using AiTesting.Domain.Common;

namespace AiTesting.Domain.Services.TestAttempt;

public interface ITestAttemptService
{
    Task<Result<Models.TestAttempt>> GetByIdAsync(Guid id);
    Task<Result> AddAsync(Models.TestAttempt testAttempt);
    Task<Result> FinishAsync(Models.TestAttempt testAttempt);
}
