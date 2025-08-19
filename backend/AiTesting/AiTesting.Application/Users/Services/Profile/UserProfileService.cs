using AiTesting.Application.Users.Dto.Profile;
using AiTesting.Domain.Common;
using AiTesting.Domain.Services.User;

namespace AiTesting.Application.Users.Services.Profile;

public class UserProfileService : IUserProfileService
{
    private readonly IUserService _userService;

    public UserProfileService(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<Result> DeleteProfile(Guid id)
    {
        return await _userService.DeleteAsync(id);
    }

    public async Task<Result<UserDto>> GetProfile(Guid id)
    {
        var userResult = await _userService.GetByIdAsync(id);

        if (userResult.IsFailure) return Result<UserDto>.Failure(userResult.Error);

        var dto = new UserDto(userResult.Value.DisplayName,
                              userResult.Value.Email);

        return Result<UserDto>.Success(dto);
    }

    public async Task<Result> UpdateProfile(Guid id, UpdateProfileDto dto)
    {
        var userResult = await _userService.GetByIdAsync(id);

        if (userResult.IsFailure) return Result.Failure("User not found");

        try
        {
            var user = userResult.Value;
            user.DisplayName = dto.Name;
            user.Email = dto.Email;
            return await _userService.UpdateAsync(user);
        }
        catch(ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
