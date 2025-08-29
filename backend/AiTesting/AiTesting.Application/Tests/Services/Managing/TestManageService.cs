using AiTesting.Application.Tests.Dto.Managing;
using AiTesting.Domain.Common;
using AiTesting.Domain.Enums;
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
    private const string QUESTIONS_IMAGES_SUBFOLDER = "tests/questions";
    private const string OPTIONS_IMAGES_SUBFOLDER = "tests/options";

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
                    CorrectAnswers: q.CorrectAnswers.Select(o => new AnswerOptionDto(
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

    public async Task<Result> UpdateMetadata(UpdateTestMetadataDto dto, Guid ownerId, string apiUrl)
    {
        var testResult = await _testService.GetByIdAsync(dto.Id);

        if (testResult.IsFailure) return testResult;

        var test = testResult.Value;
        
        if (test.CreatedById != ownerId) return Result.Failure("Cannot update test that doesn`t belong to this user");

        var coverImageUrlResult = await _fileStorageService.UploadFileAsync(dto.CoverImage, COVER_IMAGES_SUBFOLDER);
        var coverImageUrl = coverImageUrlResult.IsSuccess ?
                            coverImageUrlResult.Value :
                            string.Empty;

        try
        {
            test.Title = dto.Title;
            test.Description = dto.Description;
            test.TimeLimitMinutes = dto.TimeLimitInMinutes;
            test.IsPublic = dto.IsPublic;
            
            if (coverImageUrlResult.IsSuccess)
            {
                var oldCoverImageRelativeUrl = test.CoverImageUrl.Replace(apiUrl, string.Empty);
                test.CoverImageUrl = $"{apiUrl}{coverImageUrl}";
                await _fileStorageService.DeleteFileAsync(oldCoverImageRelativeUrl);
            }

            return await _testService.UpdateAsync(test);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result> UpdateQuestions(UpdateQuestionsDto dto, Guid ownerId, string apiUrl)
    {
        var testResult = await _testService.GetByIdAsync(dto.TestId);

        if (testResult.IsFailure) return testResult;

        var test = testResult.Value;

        if (test.CreatedById != ownerId) return Result.Failure("Cannot update test that doesn`t belong to this user");

        var questionsToAdd = dto.QuestionsToAdd.Select(
            (qdto) => UpdateQuestionDtoToQuestionAsync(qdto, test, apiUrl)
        );

        var questionsToUpdate = dto.QuestionsToUpdate.Select(
            (qdto) => UpdateQuestionDtoToQuestionAsync(qdto, test, apiUrl, false)
        );

        return await _testService.UpdateTestQuestionsAsync(
            test,
            questionsToAdd,
            questionsToUpdate,
            dto.QuestionsToDeleteIds
        );
    }

    public async Task<Result> UpdateQuestionImage(UpdateQuestionImageDto dto, Guid ownerId, string apiUrl)
    {
        var testResult = await _testService.GetByIdAsync(dto.TestId);

        if (testResult.IsFailure) return testResult;

        var test = testResult.Value;

        if (test.CreatedById != ownerId) 
            return Result.Failure("Cannot modify test that doesnt belong to this user");

        var question = test.Questions.FirstOrDefault(q => q.Id == dto.Id);

        if (question == null) 
            return Result.Failure("Question not found");

        var questionImageUrlResult = await _fileStorageService.UploadFileAsync(dto.ImageFile, QUESTIONS_IMAGES_SUBFOLDER);
        var questionImageUrl = questionImageUrlResult.IsSuccess ? questionImageUrlResult.Value : string.Empty;

        if (questionImageUrlResult.IsSuccess)
        {
            var oldImageRelativeUrl = question.ImageUrl.Replace(apiUrl, string.Empty);
            question.ImageUrl = $"{apiUrl}{questionImageUrl}";
            await _fileStorageService.DeleteFileAsync(oldImageRelativeUrl);
            await _testService.UpdateTestQuestionsAsync(test, [], [question], []);
        }

        return Result.Success();
    }

    public async Task<Result> UpdateOptionImage(UpdateOptionImageDto dto, Guid ownerId, string apiUrl)
    {
        var testResult = await _testService.GetByIdAsync(dto.TestId);

        if (testResult.IsFailure)
            return testResult;

        var test = testResult.Value;

        if (test.CreatedById != ownerId)
            return Result.Failure("Cannot modify test that doesn't belong to this user");

        var question = test.Questions.FirstOrDefault(q => q.Id == dto.QuestionId);
        if (question == null)
            return Result.Failure("Question not found");

        var option = question.Options.FirstOrDefault(o => o.Id == dto.Id);
        if (option == null)
            return Result.Failure("Option not found");

        var optionImageUrlResult = await _fileStorageService.UploadFileAsync(dto.ImageFile, OPTIONS_IMAGES_SUBFOLDER);
        var optionImageUrl = optionImageUrlResult.IsSuccess ? optionImageUrlResult.Value : string.Empty;

        if (optionImageUrlResult.IsSuccess)
        {
            var oldImageRelativeUrl = option.ImageUrl.Replace(apiUrl, string.Empty);
            option.ImageUrl = $"{apiUrl}{optionImageUrl}";
            await _fileStorageService.DeleteFileAsync(oldImageRelativeUrl);
            await _testService.UpdateTestQuestionsAsync(test, [], [question], []);
        }

        return Result.Success();
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

    private Question UpdateQuestionDtoToQuestionAsync(UpdateQuestionDto dto, Test test, string apiUrl, bool newQuestion = true)
    {
        Question question = null!;

        if (newQuestion)
        {
            var questionResult = Question.Create(
                test,
                dto.Type,
                dto.Text,
                dto.Order,
                dto.CorrectTextAnswer ?? "",
                ""
            );

            if (questionResult.IsFailure)
                throw new InvalidOperationException($"Cannot create question: {questionResult.Error}");

            question = questionResult.Value;
        }
        else
        {
            question = test.Questions.FirstOrDefault(q => q.Id == dto.Id)!;

            if (question == null)
                throw new InvalidOperationException($"Question not found");

            question.Update(
                dto.Text, 
                dto.Order, 
                question.ImageUrl, 
                dto.Type, 
                dto.CorrectTextAnswer
            );
        }

        var optionsToRemove = question.Options.Where(o => !dto.Options.Select(odto => odto.Id).Contains(o.Id));

        foreach(var optionToRemove in optionsToRemove)
            _fileStorageService.DeleteFileAsync(optionToRemove.ImageUrl.Replace(apiUrl, string.Empty)).Wait();

        question.Options.RemoveAll(o => optionsToRemove.Contains(o));
        question.CorrectAnswers.RemoveAll(ca => !dto.CorrectAnswers.Select(cadto => cadto.Id).Contains(ca.Id));

        foreach (var optionDto in dto.Options)
        {
            var existingOption = question.Options.FirstOrDefault(o => o.Id == optionDto.Id);
            if (existingOption != null)
                existingOption.Text = optionDto.Text;
            else
            {
                var newOption = AnswerOption.Create(question, optionDto.Text, "");
                if (newOption.IsSuccess)
                    question.AddOption(newOption.Value);
            }
        }

        if (question.Type != QuestionType.OpenEnded)
        {
            var correctOptions = question.Options
                .Where(o => dto.CorrectAnswers.Any(ca => ca.Id == o.Id))
                .ToList();
            question.SetCorrectAnswers(correctOptions);
        }

        return question;
    }
}
