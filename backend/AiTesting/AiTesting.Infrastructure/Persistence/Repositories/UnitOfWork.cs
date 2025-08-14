using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;
using AiTesting.Infrastructure.Persistence.Repositories.Concreate;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepository Users { get; }
    public ITestRepository Tests { get; }
    public ITestAttemptRepository TestAttempts { get; }

    private readonly DbContext _dbContext;

    public UnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
        Users = new UserRepository(dbContext);
        Tests = new TestRepository(dbContext);
        TestAttempts = new TestAttemptRepository(dbContext);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
