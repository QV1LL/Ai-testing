using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;
using AiTesting.Infrastructure.Persistence.Entities;

namespace AiTesting.Infrastructure.Persistence.Repositories.Contracts;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<Result<RefreshToken>> GetByUserIdAsync(Guid userId);
    Task<Result<RefreshToken>> GetByTokenAsync(string token);
}
