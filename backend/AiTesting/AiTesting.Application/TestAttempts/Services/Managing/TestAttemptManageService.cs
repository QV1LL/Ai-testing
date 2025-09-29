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

    public async Task<Result> AddTestAttempt(AddTestAttemptDto dto)
    {
        if ((dto.UserId != null && dto.GuestName != null) ||
            (dto.UserId == null && dto.GuestName == null))
            return Result.Failure("Test attempt cannot contain both guest name and user id and must include one of them");

        var testResult = await _testService.GetByIdAsync(dto.TestId);

        if (testResult.IsFailure) return testResult;

        Guest? guest = null;
        var userResult = await _userService.GetByIdAsync(dto.UserId ?? Guid.Empty);
        var test = testResult.Value;

        if (userResult.IsFailure)
        {
            var guestResult = Guest.Create(dto.GuestName!);

            if (guestResult.IsFailure) return guestResult;
            
            guest = guestResult.Value;
            var result = await _guestService.AddAsync(guest);

            if (result.IsFailure) return result;
        }

        var testAttemptResult = TestAttempt.Create
        (
            test, 
            userResult.IsSuccess ? userResult.Value : null, 
            guest
        );

        if (testAttemptResult.IsFailure) return testAttemptResult;

        var testAttempt = testAttemptResult.Value;

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

            if (answerResult.IsFailure) return answerResult;

            testAttempt.AddAnswer(answerResult.Value);
        }

        return await _testAttemptService.AddAndFinishAsync(testAttempt);
    }
}
