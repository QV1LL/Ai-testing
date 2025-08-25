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

    public async Task<Result<FullTestDto>> Get(Guid id)
    {
        var testResult = await _testService.GetByIdAsync(id);

        if (testResult.IsFailure) 
            return Result<FullTestDto>.Failure(testResult.Error);

        var test = testResult.Value;
        var dto = new FullTestDto(
            Id: test.Id,
            Title: test.Title,
            Description: test.Description,
            CoverImageUrl: test.CoverImageUrl,
            IsPublic: test.IsPublic,
            TimeLimitMinutes: test.TimeLimitMinutes,
            Questions: test.Questions.Select(q => new QuestionDto(
                    Id: q.Id,
                    Text: q.Text,
                    ImageUrl: q.ImageUrl,
                    Order: q.Order,
                    Type: q.Type,
                    Options: q.Options.Select(o => new AnswerOptionDto(
                        Id: o.Id,
                        Text: o.Text,
                        ImageUrl: o.ImageUrl
                    )).ToList(),
                    CorrectOptions: q.CorrectAnswers.Select(o => new AnswerOptionDto(
                        Id: o.Id,
                        Text: o.Text,
                        ImageUrl: o.ImageUrl
                    )).ToList()
            )).ToList(),
            TestAttempts: test.TestAttempts.Select(ta => new TestAttemptDto(
                    Id: ta.Id,
                    UserDisplayName: ta.User == null ? ta.Guest!.DisplayName : ta.User.DisplayName,
                    UserAvatarUrl: ta.User == null ? string.Empty : ta.User.AvatarUrl,
                    StartedAt: ta.StartedAt,
                    FinishedAt: ta.FinishedAt,
                    Score: ta.Score
            )).ToList(),
            AttemptsCount: test.TestAttempts.Count,
            AverageScore: test.TestAttempts.Count != 0 ?
                          test.TestAttempts.Average(ta => ta.Score) :
                          0
        );

        return Result<FullTestDto>.Success(dto);
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

    public async Task<Result> Delete(Guid testId, Guid ownerId, string apiUrl)
    {
        var testToDeleteResult = await _testService.GetByIdAsync(testId);

        if (testToDeleteResult.IsFailure) return testToDeleteResult;

        var testToDelete = testToDeleteResult.Value;

        if (testToDelete.CreatedById != ownerId)
            return Result.Failure("Cannot delete other user test");

        await _fileStorageService.DeleteFileAsync(testToDelete.CoverImageUrl.Replace(apiUrl, string.Empty));

        foreach(var question in testToDelete.Questions)
        {
            await _fileStorageService.DeleteFileAsync(question.ImageUrl.Replace(apiUrl, string.Empty));

            foreach(var option in question.Options)
            {
                await _fileStorageService.DeleteFileAsync(option.ImageUrl.Replace(apiUrl, string.Empty));
            }
        }

        var result = await _testService.DeleteAsync(testId);

        return result;
    }
}
