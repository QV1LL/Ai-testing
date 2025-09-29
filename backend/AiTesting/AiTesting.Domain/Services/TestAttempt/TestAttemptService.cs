using AiTesting.Domain.Common;
using AiTesting.Domain.Common.Specifications.Test;
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

    public async Task<Result> AddAndFinishAsync(Models.TestAttempt testAttempt)
    {
        var test = await _testRepository.GetByIdAsync(testAttempt.Test.Id, new TestWithQuestionsSpecification());

        if (test == null)
            return Result.Failure("Test wasn`t found");

        var score = CalculateScore(testAttempt, test);
        testAttempt.Finish(score);

        await _repository.AddAsync(testAttempt);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    private static double CalculateScore(Models.TestAttempt testAttempt, Models.Test test)
    {
        if (test == null || test.Questions == null || test.Questions.Count == 0)
            return 0;

        int totalCorrectAnswers = 0;
        int userCorrectAnswers = 0;

        foreach (var question in test.Questions)
        {
            var userAnswer = testAttempt.Answers.FirstOrDefault(a => a.Question.Id == question.Id);
            if (question.CorrectAnswers == null) continue;

            var correctIds = question.CorrectAnswers.Select(o => o.Id).ToList();
            totalCorrectAnswers += correctIds.Count;

            if (userAnswer == null) continue;

            if (question.Type == Enums.QuestionType.SingleChoice || question.Type == Enums.QuestionType.MultipleChoice)
            {
                if (userAnswer.SelectedOptions == null) continue;
                userCorrectAnswers += userAnswer.SelectedOptions.Select(o => o.Id)
                                                                .Count(correctIds.Contains);
            }
            else if (question.Type == Enums.QuestionType.OpenEnded)
            {
                if (!string.IsNullOrWhiteSpace(question.CorrectTextAnswer) &&
                    string.Equals(userAnswer.WrittenAnswer?.Trim(), question.CorrectTextAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    userCorrectAnswers++;
                    totalCorrectAnswers++;
                }
                else
                {
                    totalCorrectAnswers++;
                }
            }
        }

        if (totalCorrectAnswers == 0) return 0;
        return (double)userCorrectAnswers / totalCorrectAnswers * 100.0;
    }

}
