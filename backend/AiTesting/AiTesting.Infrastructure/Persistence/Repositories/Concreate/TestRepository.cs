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
}
