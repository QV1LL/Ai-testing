using AiTesting.Application.Users.Dto.Profile;
using AiTesting.Domain.Common;
using AiTesting.Domain.Services.User;
using AiTesting.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Users.Services.Profile;

public class UserProfileService : IUserProfileService
{
    private readonly IUserService _userService;
    private readonly IFileStorageService _fileStorageService;

    private const string AVATAR_IMAGES_SUBFOLDER = "users/avatars";

    public UserProfileService(IUserService userService, 
                              IFileStorageService fileStorageService)
    {
        _userService = userService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> DeleteProfile(Guid id, string apiUrl)
    {
        var userResult = await _userService.GetByIdAsync(id);

        if (userResult.IsFailure) return userResult;

        await _fileStorageService.DeleteFileAsync(userResult.Value.AvatarUrl.Replace(apiUrl, string.Empty));
        return await _userService.DeleteAsync(id);
    }

    public async Task<Result<UserDto>> GetProfile(Guid id)
    {
        var userResult = await _userService.GetByIdAsync(id);

        if (userResult.IsFailure) return Result<UserDto>.Failure(userResult.Error);

        var user = userResult.Value;
        var dto = new UserDto(
            user.DisplayName,
            user.Email,
            user.AvatarUrl,
            user.Tests.Select(
                t => new TestProfileDto(t.Id, t.Title, t.CreatedAt)).ToList(),
            user.TestAttempts.Select(
                ta => new TestAttemptProfileDto(ta.Id, ta.Test.Title, ta.StartedAt, ta.Score)).ToList()
        );
        
        return Result<UserDto>.Success(dto);
    }

    public async Task<Result> UpdateProfile(Guid id, UpdateProfileDto dto, IFormFile? avatarImage, string apiUrl)
    {
        var userResult = await _userService.GetByIdAsync(id);

        if (userResult.IsFailure) return Result.Failure("User not found");

        var avatarImageUrlResult = await _fileStorageService.UploadFileAsync(avatarImage, AVATAR_IMAGES_SUBFOLDER);
        var avatarImageUrl = avatarImageUrlResult.IsSuccess ?
                            avatarImageUrlResult.Value :
                            string.Empty;

        try
        {
            var user = userResult.Value;
            user.DisplayName = dto.Name;
            user.Email = dto.Email;

            if (avatarImageUrlResult.IsSuccess)
            {
                var oldAvatarRelativeUrl = user.AvatarUrl.Replace(apiUrl, string.Empty);
                user.AvatarUrl = $"{apiUrl}{avatarImageUrl}";
                await _fileStorageService.DeleteFileAsync(oldAvatarRelativeUrl);
            }

            return await _userService.UpdateAsync(user);

        }
        catch(ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
