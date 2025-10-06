using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using AiTesting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories.Concreate;

public class TestRepository(DbContext dbContext) : GenericRepository<Test>(dbContext), ITestRepository
{
    public async Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<Test>()
                       .Where(t => t.Id == id)
                       .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<Test?> GetByJoinIdAsync(string joinId, ISpecification<Test> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification);
        return await query.FirstOrDefaultAsync(e => e.JoinId == joinId, cancellationToken);
    }

    public async Task<bool> IsJoinIdExist(string joinId)
    {
        if (string.IsNullOrWhiteSpace(joinId))
            return false;

        return await DbContext.Set<Test>()
                              .AsNoTracking()
                              .AnyAsync(t => t.JoinId == joinId);
    }
}
