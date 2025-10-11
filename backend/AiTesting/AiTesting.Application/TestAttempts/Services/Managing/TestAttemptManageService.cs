using AiTesting.Application.TestAttempts.Dto.Managing;
using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using AiTesting.Domain.Services.Guest;
using AiTesting.Domain.Services.Test;
using AiTesting.Domain.Services.TestAttempt;
using AiTesting.Domain.Services.User;

namespace AiTesting.Application.TestAttempts.Services.Managing;

internal class TestAttemptManageService : ITestAttemptManageService
{
    private readonly ITestAttemptService _testAttemptService;
    private readonly ITestService _testService;
    private readonly IGuestService _guestService;
    private readonly IUserService _userService;

    public TestAttemptManageService(ITestAttemptService testAttemptService,
                                    IGuestService guestService,
                                    IUserService userService,
                                    ITestService testService)
    {
        _testAttemptService = testAttemptService;
        _guestService = guestService;
        _userService = userService;
        _testService = testService;
    }

    public async Task<Result<TestAttemptMetadataDto>> GetById(Guid id)
    {
        var testAttemptResult = await _testAttemptService.GetByIdAsync(id);

        if (testAttemptResult.IsFailure)
            return Result<TestAttemptMetadataDto>.Failure(testAttemptResult.Error);

        var testAttempt = testAttemptResult.Value;
        var testAttemptMetadataDto = new TestAttemptMetadataDto
        (
            TestId: testAttempt.TestId,
            GuestName: testAttempt.Guest?.DisplayName
        );

        return Result<TestAttemptMetadataDto>.Success(testAttemptMetadataDto);
    }

    public async Task<Result<AddTestAttemptResultDto>> AddTestAttempt(AddTestAttemptDto dto)
    {
        if ((dto.UserId != null && dto.GuestName != null) ||
            (dto.UserId == null && dto.GuestName == null))
            return Result<AddTestAttemptResultDto>.Failure("Test attempt cannot contain both guest name and user id and must include one of them");

        var testResult = await _testService.GetByJoinIdAsync(dto.TestJoinId);

        if (testResult.IsFailure) 
            return Result<AddTestAttemptResultDto>.Failure(testResult.Error);

        Guest? guest = null;
        var userResult = await _userService.GetByIdAsync(dto.UserId ?? Guid.Empty);
        var test = testResult.Value;

        if (userResult.IsFailure)
        {
            var guestResult = Guest.Create(dto.GuestName!);

            if (guestResult.IsFailure) 
                return Result<AddTestAttemptResultDto>.Failure(guestResult.Error);
            
            guest = guestResult.Value;
            var result = await _guestService.AddAsync(guest);

            if (result.IsFailure) 
                return Result<AddTestAttemptResultDto>.Failure(result.Error);
        }

        var testAttemptResult = TestAttempt.Create
        (
            test, 
            userResult.IsSuccess ? userResult.Value : null, 
            guest
        );

        if (testAttemptResult.IsFailure) 
            return Result<AddTestAttemptResultDto>.Failure(testAttemptResult.Error);

        var testAttempt = testAttemptResult.Value;
        var addResult = await _testAttemptService.AddAsync(testAttempt);

        if (addResult.IsFailure)
            return Result<AddTestAttemptResultDto>.Failure(addResult.Error);

        var addTestAttemptResultDto = new AddTestAttemptResultDto(AttemptId: testAttempt.Id);

        return Result<AddTestAttemptResultDto>.Success(addTestAttemptResultDto);
    }

    public async Task<Result<TestAttemptResultDto>> FinishTestAttempt(FinishTestAttemptDto dto)
    {
        var testAttemptResult = await _testAttemptService.GetByIdAsync(dto.AttemptId);

        if (testAttemptResult.IsFailure)
            return Result<TestAttemptResultDto>.Failure(testAttemptResult.Error);

        var testAttempt = testAttemptResult.Value;
        var testResult = await _testService.GetByIdAsync(testAttempt.TestId);

        if (testResult.IsFailure)
            return Result<TestAttemptResultDto>.Failure(testResult.Error);

        var test = testResult.Value;

        foreach (var answerDto in dto.Answers)
        {
            var answerResult = AttemptAnswer.Create
            (
                attempt: testAttempt,
                question: test.Questions.FirstOrDefault(q => q.Id == answerDto.QuestionId),
                selectedOptions: test.Questions.SelectMany(q => q.Options)
                                               .Where(o => answerDto.SelectedOptions.Select(so => so.Id).Contains(o.Id))
                                               .ToList(),
                writtenAnswer: answerDto.WrittenAnswer
            );

            if (answerResult.IsFailure)
                return Result<TestAttemptResultDto>.Failure(answerResult.Error);

            testAttempt.AddAnswer(answerResult.Value);
        }

        var finishResult = await _testAttemptService.FinishAsync(testAttempt);

        if (finishResult.IsFailure)
            return Result<TestAttemptResultDto>.Failure(finishResult.Error);

        var testAttemptResultDto = new TestAttemptResultDto
        (
            TestTitle: test.Title,
            DisplayUsername: testAttempt.User == null ?
                             testAttempt.Guest.DisplayName :
                             testAttempt.User.DisplayName,
            StartedAt: testAttempt.StartedAt,
            FinishedAt: testAttempt.FinishedAt ?? new(),
            Score: testAttempt.Score
        );

        return Result<TestAttemptResultDto>.Success(testAttemptResultDto);
    }
}
