using AiTesting.Domain.Repositories;

namespace AiTesting.Domain.Common;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ITestRepository Tests { get; }
    ITestAttemptRepository TestAttempts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}