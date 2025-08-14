using AiTesting.Domain.Models;

namespace AiTesting.Domain.Repositories;

public interface ITestAttemptRepository : IRepository<TestAttempt>
{
    Task<IReadOnlyList<TestAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}