using AiTesting.Application.Users.Dto.Auth;
using AiTesting.Domain.Common;

namespace AiTesting.Application.Users.Services.Auth;

public interface IAuthService
{
    Task<Result<AuthResultDto>> Register(RegisterUserDto dto);
    Task<Result<AuthResultDto>> Login(LoginDto dto);
    Task<Result<AuthResultDto>> Refresh(RefreshTokenDto dto);
}
