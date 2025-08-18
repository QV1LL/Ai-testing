using AiTesting.Domain.Models;
using AiTesting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories.Concreate;

public class UserRepository(DbContext dbContext) : GenericRepository<User>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>()
                              .FirstOrDefaultAsync(u => u.Email == email, cancellationToken: cancellationToken);
    }
}
