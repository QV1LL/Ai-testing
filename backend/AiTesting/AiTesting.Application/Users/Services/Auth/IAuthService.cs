using AiTesting.Application.Users.Dto.Auth;
using AiTesting.Domain.Common;

namespace AiTesting.Application.Users.Services.Auth;

public interface IAuthService
{
    Task<Result<LoginResultDto>> Register(RegisterUserDto dto);
    Task<Result<LoginResultDto>> Login(LoginDto dto);
}
