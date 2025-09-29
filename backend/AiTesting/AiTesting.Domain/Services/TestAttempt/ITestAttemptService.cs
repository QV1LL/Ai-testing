using AiTesting.Domain.Common;

namespace AiTesting.Domain.Services.TestAttempt;

public interface ITestAttemptService
{
    Task<Result> AddAndFinishAsync(Models.TestAttempt testAttempt);
}
