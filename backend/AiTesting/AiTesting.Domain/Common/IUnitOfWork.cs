using AiTesting.Domain.Repositories;

namespace AiTesting.Domain.Common;

public interface IUnitOfWork
{
    IGuestRepository Guests { get; }
    IUserRepository Users { get; }
    ITestRepository Tests { get; }
    IQuestionRepository Questions { get; }
    ITestAttemptRepository TestAttempts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}