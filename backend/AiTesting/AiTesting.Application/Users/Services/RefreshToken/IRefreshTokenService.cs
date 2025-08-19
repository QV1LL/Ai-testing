using AiTesting.Domain.Common;

namespace AiTesting.Application.Users.Services.RefreshToken;

public interface IRefreshTokenService
{
    Task<Result<Infrastructure.Persistence.Entities.RefreshToken>> GetByUserIdAsync(Guid userId);
    Task<Result<Infrastructure.Persistence.Entities.RefreshToken>> GetByTokenAsync(string token);
    Task<Result> SaveAsync(Infrastructure.Persistence.Entities.RefreshToken token);
}
