using AiTesting.Domain.Common;
using AiTesting.Infrastructure.Persistence.Entities;
using AiTesting.Infrastructure.Persistence.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories.Concreate;

public class RefreshTokenRepository(DbContext dbContext) : GenericRepository<RefreshToken>(dbContext), IRefreshTokenRepository
{
    public override Task DeleteAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException();
    }

    public async Task<Result<RefreshToken>> GetByTokenAsync(string token)
    {
        var refreshToken = await DbContext.Set<RefreshToken>()
                                           .FirstOrDefaultAsync(rt => rt.Token == token);

        return refreshToken == null ?
               Result<RefreshToken>.Failure("Refresh token not found") :
               Result<RefreshToken>.Success(refreshToken);
    }

    public async Task<Result<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        var token = await DbContext.Set<RefreshToken>()
                                   .FirstOrDefaultAsync(rt => rt.UserId == userId);

        return token == null ? 
               Result<RefreshToken>.Failure($"Token with user id '{userId}' was not found") :
               Result<RefreshToken>.Success(token);
    }
}
