using AiTesting.Domain.Common;
using AiTesting.Domain.Common.Specifications.Test;
using AiTesting.Domain.Common.Specifications.TestAttempt;
using AiTesting.Domain.Repositories;

namespace AiTesting.Domain.Services.TestAttempt;

internal class TestAttemptService : ITestAttemptService
{
    private readonly ITestAttemptRepository _repository;
    private readonly ITestRepository _testRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TestAttemptService(IUnitOfWork unitOfWork, 
                              ITestAttemptRepository repository, 

                              ITestRepository testRepository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _testRepository = testRepository;
    }

    public async Task<Result<Models.TestAttempt>> GetByIdAsync(Guid id)
    {
        var testAttempt = await _repository.GetByIdAsync(id, new TestAttemptWithGuest());

        if (testAttempt == null || testAttempt.FinishedAt != null)
            return Result<Models.TestAttempt>.Failure("Attempt not found");

        return Result<Models.TestAttempt>.Success(testAttempt);
    }

    public async Task<Result> AddAsync(Models.TestAttempt testAttempt)
    {
        await _repository.AddAsync(testAttempt);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Models.TestAttempt testAttempt)
    {
        await _repository.UpdateAsync(testAttempt);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> FinishAsync(Models.TestAttempt testAttempt)
    {
        var test = await _testRepository.GetByIdAsync(testAttempt.Test.Id, new TestWithQuestionsSpecification());

        if (test == null)
        {
            await _repository.DeleteAsync(testAttempt);
            await _unitOfWork.SaveChangesAsync();
            return Result.Failure("Test wasn`t found, attempt failed");
        }

        var attemptTime = (DateTimeOffset.Now - testAttempt.StartedAt).TotalSeconds;
        var maxAttemptTime = test.TimeLimitMinutes == null ? test.TimeLimitMinutes * 60 : int.MaxValue;

        if (attemptTime > maxAttemptTime)
        {
            await _repository.DeleteAsync(testAttempt);
            await _unitOfWork.SaveChangesAsync();
            return Result.Failure("Test attempt is expired, attempt failed");
        }

        var score = CalculateScore(testAttempt, test);
        testAttempt.Finish(score);

        await _repository.UpdateAsync(testAttempt);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    private static double CalculateScore(Models.TestAttempt testAttempt, Models.Test test)
    {
        if (test == null || test.Questions == null || test.Questions.Count == 0)
            return 0;

        double totalScore = 0;
        double userScore = 0;

        foreach (var question in test.Questions)
        {
            var userAnswer = testAttempt.Answers.FirstOrDefault(a => a.Question.Id == question.Id);
            if (question.CorrectAnswers == null && question.Type != Enums.QuestionType.OpenEnded)
                continue;

            totalScore += 1.0;

            if (userAnswer == null)
                continue;

            switch (question.Type)
            {
                case Enums.QuestionType.SingleChoice:
                    {
                        var correctId = question.CorrectAnswers.FirstOrDefault()?.Id;
                        var selectedId = userAnswer.SelectedOptions?.FirstOrDefault()?.Id;
                        if (correctId != null && selectedId == correctId) userScore += 1.0;

                        break;
                    }

                case Enums.QuestionType.MultipleChoice:
                    {
                        var correctIds = question.CorrectAnswers.Select(o => o.Id).ToHashSet();
                        var selectedIds = userAnswer.SelectedOptions?.Select(o => o.Id).ToHashSet() ?? [];

                        bool hasWrongSelection = selectedIds.Except(correctIds).Any();
                        if (hasWrongSelection) break;

                        int correctlySelected = selectedIds.Count(correctIds.Contains);
                        int totalCorrect = correctIds.Count;

                        double partialScore = (double)correctlySelected / totalCorrect;
                        userScore += partialScore;
                        break;
                    }

                case Enums.QuestionType.OpenEnded:
                    {
                        if (!string.IsNullOrWhiteSpace(question.CorrectTextAnswer) &&
                            string.Equals(userAnswer.WrittenAnswer?.Trim(), question.CorrectTextAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                            userScore += 1.0;

                        break;
                    }
            }
        }

        return totalScore <= 0 ? 
               0 : 
               userScore / totalScore * 100.0;
    }
}
