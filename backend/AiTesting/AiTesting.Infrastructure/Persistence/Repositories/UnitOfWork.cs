using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public IGuestRepository Guests { get; }
    public IUserRepository Users { get; }
    public ITestRepository Tests { get; }
    public IQuestionRepository Questions { get; }
    public ITestAttemptRepository TestAttempts { get; }

    private readonly DbContext _dbContext;

    public UnitOfWork(DbContext dbContext,
                      IGuestRepository guestRepository,
                      IUserRepository userRepository,
                      ITestRepository testRepository,
                      ITestAttemptRepository testAttemptRepository,
                      IQuestionRepository questions)
    {
        _dbContext = dbContext;
        Guests = guestRepository;
        Users = userRepository;
        Tests = testRepository;
        TestAttempts = testAttemptRepository;
        Questions = questions;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
