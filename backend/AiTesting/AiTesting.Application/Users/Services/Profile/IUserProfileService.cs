using AiTesting.Application.Users.Dto.Profile;
using AiTesting.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Users.Services.Profile;

public interface IUserProfileService
{
    Task<Result<UserDto>> GetProfile(Guid id);
    Task<Result> UpdateProfile(Guid id, UpdateProfileDto dto, IFormFile? avatarImage, string apiUrl);
    Task<Result> DeleteProfile(Guid id);
}
