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

    public async Task<Result<UserDto>> GetProfile(Guid id)
    {
        var userResult = await _userService.GetAsync(id);

        if (userResult.IsFailure) return Result<UserDto>.Failure(userResult.Error);

        var dto = new UserDto(userResult.Value.DisplayName,
                              userResult.Value.Email);

        return Result<UserDto>.Success(dto);
    }
}
