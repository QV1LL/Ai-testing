using AiTesting.Application.Tests.Dto.Managing;
using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using AiTesting.Domain.Services.Test;
using AiTesting.Domain.Services.User;
using AiTesting.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Tests.Services.Managing;

internal class TestManageService : ITestManageService
{
    private readonly ITestService _testService;
    private readonly IUserService _userService;
    
    private readonly IFileStorageService _fileStorageService;

    private const string COVER_IMAGES_SUBFOLDER = "tests/coverImages";

    public TestManageService(IUserService userService,
                             ITestService testService,
                             IFileStorageService fileStorageService)
    {
        _userService = userService;
        _testService = testService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<UserTestsResultDto>> GetUserTests(Guid ownerId)
    {
        var userResult = await _userService.GetByIdAsync(ownerId);

        if (userResult.IsFailure)
            return Result<UserTestsResultDto>.Failure("User not found");

        var user = userResult.Value;
        var dto = new UserTestsResultDto(
            Tests: [..user.Tests.Select(t => new TestDto(t.Id, t.Title, t.Description))]
        );

        return Result<UserTestsResultDto>.Success(dto);
    }

    public async Task<Result<CreateTestResultDto>> Create(CreateTestDto dto, IFormFile? coverImage, Guid ownerId, string apiUrl)
    {
        var userResult = await _userService.GetByIdAsync(ownerId);

        if (userResult.IsFailure)
            return Result<CreateTestResultDto>.Failure("Test must have an owner");

        var user = userResult.Value;
        var coverImageRelativeUrlResult = await _fileStorageService.UploadFileAsync(coverImage, COVER_IMAGES_SUBFOLDER);
        var coverImageUrl = coverImageRelativeUrlResult.IsSuccess ? 
                            $"{apiUrl}{coverImageRelativeUrlResult.Value}" : 
                            string.Empty;

        var testResult = Test.Create(
            title: dto.Title,
            description: dto.Description,
            createdBy: user,
            isPublic: dto.IsPublic,
            coverImageUrl: coverImageUrl,
            timeLimitMinutes: dto.TimeLimitMinutes
        );

        if (testResult.IsFailure)
            return Result<CreateTestResultDto>.Failure(testResult.Error);

        var test = testResult.Value;
        var result = await _testService.AddAsync(test);

        if (result.IsFailure)
            return Result<CreateTestResultDto>.Failure(result.Error);

        var createTestResult = new CreateTestResultDto(
            Id: test.Id,
            Title: test.Title,
            Description: test.Description
        );

        return Result<CreateTestResultDto>.Success(createTestResult);
    }
}
