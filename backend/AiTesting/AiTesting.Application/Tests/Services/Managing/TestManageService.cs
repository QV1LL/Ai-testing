using AiTesting.Application.Tests.Dto.Managing;
using AiTesting.Domain.Common;
using AiTesting.Domain.Enums;
using AiTesting.Domain.Models;
using AiTesting.Domain.Services.Test;
using AiTesting.Domain.Services.User;
using AiTesting.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using System.Net;

namespace AiTesting.Application.Tests.Services.Managing;

internal class TestManageService : ITestManageService
{
    private readonly ITestService _testService;
    private readonly IUserService _userService;
    
    private readonly IFileStorageService _fileStorageService;
    private readonly Infrastructure.Services.Http.IHttpContextAccessor _httpContextAccessor;

    private const string COVER_IMAGES_SUBFOLDER = "tests/coverImages";
    private const string QUESTIONS_IMAGES_SUBFOLDER = "tests/questions";
    private const string OPTIONS_IMAGES_SUBFOLDER = "tests/options";

    public TestManageService(IUserService userService,
                             ITestService testService,
                             IFileStorageService fileStorageService,
                             Infrastructure.Services.Http.IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _testService = testService;
        _fileStorageService = fileStorageService;
        _httpContextAccessor = httpContextAccessor;
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
            CoverImageUrl: string.IsNullOrEmpty(test.CoverImageUrl) ? test.CoverImageUrl : $"{_httpContextAccessor.GetApiUrl()}{test.CoverImageUrl}",
            IsPublic: test.IsPublic,
            TimeLimitMinutes: test.TimeLimitMinutes,
            Questions: test.Questions.Select(q => new QuestionDto(
                    Id: q.Id,
                    Text: q.Text,
                    ImageUrl: string.IsNullOrEmpty(q.ImageUrl) ? q.ImageUrl : $"{_httpContextAccessor.GetApiUrl()}{q.ImageUrl}",
                    Order: q.Order,
                    Type: q.Type,
                    Options: q.Options.Select(o => new AnswerOptionDto(
                        Id: o.Id,
                        Text: o.Text,
                        ImageUrl: string.IsNullOrEmpty(o.ImageUrl) ? o.ImageUrl : $"{_httpContextAccessor.GetApiUrl()}{o.ImageUrl}"
                    )).ToList(),
                    CorrectAnswers: q.CorrectAnswers.Select(o => new AnswerOptionDto(
                        Id: o.Id,
                        Text: o.Text,
                        ImageUrl: string.IsNullOrEmpty(o.ImageUrl) ? o.ImageUrl : $"{_httpContextAccessor.GetApiUrl()}{o.ImageUrl}"
                    )).ToList()
            )).ToList(),
            TestAttempts: test.TestAttempts.Select(ta => new TestAttemptDto(
                    Id: ta.Id,
                    UserDisplayName: ta.User == null ? ta.Guest!.DisplayName : ta.User.DisplayName,
                    UserAvatarUrl: ta.User == null ? string.Empty : (string.IsNullOrEmpty(ta.User.AvatarUrl) ? ta.User.AvatarUrl : $"{_httpContextAccessor.GetApiUrl()}{ta.User.AvatarUrl}"),
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

    public async Task<Result<CreateTestResultDto>> Create(CreateTestDto dto, IFormFile? coverImage, Guid ownerId)
    {
        var userResult = await _userService.GetByIdAsync(ownerId);

        if (userResult.IsFailure)
            return Result<CreateTestResultDto>.Failure("Test must have an owner");

        var user = userResult.Value;
        var coverImageRelativeUrlResult = await _fileStorageService.UploadFileAsync(coverImage, COVER_IMAGES_SUBFOLDER);
        var coverImageUrl = coverImageRelativeUrlResult.IsSuccess ? 
                            coverImageRelativeUrlResult.Value : 
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

    public async Task<Result> UpdateMetadata(UpdateTestMetadataDto dto, Guid ownerId)
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
                var oldCoverImageUrl = test.CoverImageUrl;
                test.CoverImageUrl = coverImageUrl;
                await _fileStorageService.DeleteFileAsync(oldCoverImageUrl);
            }

            return await _testService.UpdateAsync(test);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result<UpdateQuestionsResultDto>> UpdateQuestions(UpdateQuestionsDto dto, Guid ownerId)
    {
        var testResult = await _testService.GetByIdAsync(dto.TestId);

        if (testResult.IsFailure) 
            return Result<UpdateQuestionsResultDto>.Failure(testResult.Error);

        var test = testResult.Value;

        if (test.CreatedById != ownerId) 
            return Result<UpdateQuestionsResultDto>.Failure("Cannot update test that doesn`t belong to this user");

        var updateQuestionsResultDto = new UpdateQuestionsResultDto([], []);

        var questionsToAdd = dto.QuestionsToAdd.Select((qdto) =>
        {
            var questionResult = UpdateQuestionDtoToQuestionAsync(qdto, test);

            if (questionResult.IsFailure) return null;

            var question = questionResult.Value;

            updateQuestionsResultDto.QuestionTempToRegularIds.Add(qdto.Id, question.Id);
            AddQuestionOptionsIdsToDto(ref updateQuestionsResultDto, qdto, question);

            return question;
        });

        var questionsToUpdate = dto.QuestionsToUpdate.Select((qdto) => {
            var questionResult = UpdateQuestionDtoToQuestionAsync(qdto, test, false);

            if (questionResult.IsFailure) return null;

            var question = questionResult.Value;

            AddQuestionOptionsIdsToDto(ref updateQuestionsResultDto, qdto, question);

            return question;
        });

        var imageUrlsToDelete = test.Questions
            .Where(q => q.ImageUrl != null && dto.QuestionsToDeleteIds.Contains(q.Id))
            .Select(q => q.ImageUrl!)
            .Concat(
                test.Questions
                    .SelectMany(q => q.Options)
                    .Where(o => o.ImageUrl != null)
                    .Select(o => o.ImageUrl!)
            )
            .ToList();

        var updateQuestionsResult = await _testService.UpdateTestQuestionsAsync(
            test,
            questionsToAdd!,
            questionsToUpdate!,
            dto.QuestionsToDeleteIds
        );

        if (updateQuestionsResult.IsFailure)
            return Result<UpdateQuestionsResultDto>.Failure(updateQuestionsResult.Error);

        var deleteResult = await DeleteQuestionsAndOptionsImages(imageUrlsToDelete);

        if (deleteResult.IsFailure)
            return Result<UpdateQuestionsResultDto>.Failure(deleteResult.Error);

        return Result<UpdateQuestionsResultDto>.Success(updateQuestionsResultDto);
    }

    public async Task<Result> UpdateQuestionImage(UpdateQuestionImageDto dto, Guid ownerId)
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
            var oldImageUrl = question.ImageUrl;
            question.ImageUrl = questionImageUrl;
            await _fileStorageService.DeleteFileAsync(oldImageUrl);
            await _testService.UpdateTestQuestionsAsync(test, [], [question], []);
        }

        return Result.Success();
    }

    public async Task<Result> UpdateOptionImage(UpdateOptionImageDto dto, Guid ownerId)
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
            var oldImageRelativeUrl = option.ImageUrl;
            option.ImageUrl = optionImageUrl;
            await _fileStorageService.DeleteFileAsync(oldImageRelativeUrl);
            await _testService.UpdateTestQuestionsAsync(test, [], [question], []);
        }

        return Result.Success();
    }

    public async Task<Result> Delete(Guid testId, Guid ownerId)
    {
        var testToDeleteResult = await _testService.GetByIdAsync(testId);

        if (testToDeleteResult.IsFailure) return testToDeleteResult;

        var testToDelete = testToDeleteResult.Value;

        if (testToDelete.CreatedById != ownerId)
            return Result.Failure("Cannot delete other user test");

        await _fileStorageService.DeleteFileAsync(testToDelete.CoverImageUrl);

        foreach(var question in testToDelete.Questions)
        {
            await _fileStorageService.DeleteFileAsync(question.ImageUrl);

            foreach(var option in question.Options)
            {
                await _fileStorageService.DeleteFileAsync(option.ImageUrl);
            }
        }

        var result = await _testService.DeleteAsync(testId);

        return result;
    }

    private Result<Question> UpdateQuestionDtoToQuestionAsync(UpdateQuestionDto dto, Test test, bool newQuestion = true)
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
                return Result<Question>.Failure(questionResult.Error);

            question = questionResult.Value;
        }
        else
        {
            question = test.Questions.FirstOrDefault(q => q.Id == dto.Id)!;

            if (question == null)
                return Result<Question>.Failure("Question not found");

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
            _fileStorageService.DeleteFileAsync(optionToRemove.ImageUrl).Wait();

        question.Options.RemoveAll(o => optionsToRemove.Contains(o));
        question.CorrectAnswers.RemoveAll(ca => !dto.CorrectAnswers.Select(cadto => cadto.Id).Contains(ca.Id));

        List<AnswerOption> correctAnswers = [];

        foreach (var optionDto in dto.Options)
        {
            var existingOption = question.Options.FirstOrDefault(o => o.Id == optionDto.Id);
            if (existingOption != null)
            {
                existingOption.Text = optionDto.Text;

                if (dto.CorrectAnswers.Any(ca => ca.Id == optionDto.Id))
                    correctAnswers.Add(existingOption);
            }
            else
            {
                var newOption = AnswerOption.Create(question, optionDto.Text);
                if (newOption.IsSuccess)
                {
                    var option = newOption.Value;
                    question.AddOption(option);

                    if (dto.CorrectAnswers.Any(ca => ca.Id == optionDto.Id))
                        correctAnswers.Add(option);
                }
            }
        }

        if (question.Type != QuestionType.OpenEnded)
            question.SetCorrectAnswers(correctAnswers);

        return Result<Question>.Success(question);
    }

    private async Task<Result> DeleteQuestionsAndOptionsImages(IEnumerable<string> imageUrlsToDelete)
    {
        foreach(var imageUrl in imageUrlsToDelete)
        {
            if (string.IsNullOrEmpty(imageUrl)) continue;

            var result = await _fileStorageService.DeleteFileAsync(imageUrl);

            if (result.IsFailure) return result;
        }

        return Result.Success();
    }

    private static void AddQuestionOptionsIdsToDto(ref UpdateQuestionsResultDto dto, UpdateQuestionDto qdto, Question question)
    {
        var updateQuestionDtoOptions = qdto.Options;
        var questionOptions = question.Options;

        foreach (var pair in updateQuestionDtoOptions.Zip(
            questionOptions,
            (uqdo, qo) => new { TempId = uqdo.Id, RegularId = qo.Id }))
        {
            dto.OptionTempToRegularIds.Add(pair.TempId, pair.RegularId);
        }
    }
}
