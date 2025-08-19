using AiTesting.Application.Users.Dto.Profile;
using AiTesting.Domain.Common;

namespace AiTesting.Application.Users.Services.Profile;

public interface IUserProfileService
{
    Task<Result<UserDto>> GetProfile(Guid id);
    Task<Result> UpdateProfile(Guid id, UpdateProfileDto dto);
    Task<Result> DeleteProfile(Guid id);
}
