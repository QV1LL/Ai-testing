using AiTesting.Domain.Common;
using AiTesting.Infrastructure.Persistence.Repositories.Contracts;

namespace AiTesting.Application.Users.Services.RefreshToken;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, 
                               IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Infrastructure.Persistence.Entities.RefreshToken>> GetByTokenAsync(string token)
    {
        return await _refreshTokenRepository.GetByTokenAsync(token);
    }

    public async Task<Result<Infrastructure.Persistence.Entities.RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        return await _refreshTokenRepository.GetByUserIdAsync(userId);
    }

    public async Task<Result> SaveAsync(Infrastructure.Persistence.Entities.RefreshToken token)
    {
        var tokenExistResult = await _refreshTokenRepository.GetByUserIdAsync(token.UserId);

        if (tokenExistResult.IsFailure) await _refreshTokenRepository.AddAsync(token);
        else await _refreshTokenRepository.UpdateAsync(token);

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
