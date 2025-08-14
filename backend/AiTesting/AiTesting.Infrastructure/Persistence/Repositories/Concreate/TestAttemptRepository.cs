using AiTesting.Domain.Models;
using AiTesting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories.Concreate;

public class TestAttemptRepository(DbContext dbContext) : GenericRepository<TestAttempt>(dbContext), ITestAttemptRepository
{
    public async Task<IReadOnlyList<TestAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TestAttempt>()
                              .Where(t => t.UserId == userId)
                              .AsNoTracking()
                              .ToListAsync(cancellationToken);
    }
}
