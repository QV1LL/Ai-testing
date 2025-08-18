using AiTesting.Application.Users.Dto.Profile;
using AiTesting.Domain.Common;

namespace AiTesting.Application.Users.Services.Profile;

public interface IUserProfileService
{
    Task<Result<UserDto>> GetProfile(Guid id);
}
