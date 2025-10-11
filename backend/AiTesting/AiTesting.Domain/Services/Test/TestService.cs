using AiTesting.Domain.Common;
using AiTesting.Domain.Common.Specifications;
using AiTesting.Domain.Common.Specifications.Test;
using AiTesting.Domain.Models;
using AiTesting.Domain.Repositories;

namespace AiTesting.Domain.Services.Test;

internal class TestService : ITestService
{
    private readonly IQuestionRepository _questionsRepository;
    private readonly ITestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TestService(ITestRepository repository, 
                       IUnitOfWork unitOfWork,
                       IQuestionRepository questionsRepository)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _questionsRepository = questionsRepository;
    }

    public async Task<Result<Models.Test>> GetByIdAsync(Guid id)
    {
        var test = await _repository.GetByIdAsync(id, new TestWithFullQuestionsAndTestAttemptsSpecification());

        return test == null ?
               Result<Models.Test>.Failure("Test not found") :
               Result<Models.Test>.Success(test);
    }

    public async Task<Result<Models.Test>> GetByJoinIdAsync(string id)
    {
        var test = await _repository.GetByJoinIdAsync(id, new TestWithFullQuestionsAndTestAttemptsSpecification());

        return test == null ?
               Result<Models.Test>.Failure("Test not found") :
               Result<Models.Test>.Success(test);
    }

    public async Task<Result<Models.Test>> GetMetadataByJoinIdAsync(string id)
    {
        var test = await _repository.GetByJoinIdAsync(id, new DefaultSpecification<Models.Test>());

        return test == null ?
               Result<Models.Test>.Failure("Test not found") :
               Result<Models.Test>.Success(test);
    }

    public async Task<Result> AddAsync(Models.Test test)
    {
        await EnsureUniqueJoinId(test);

        await _repository.AddAsync(test);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await _repository.DeleteByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Models.Test test)
    {
        await _repository.UpdateAsync(test);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateTestQuestionsAsync(
        Models.Test test,
        IEnumerable<Question> questionsToAdd,
        IEnumerable<Question> questionsToUpdate,
        IEnumerable<Guid> questionsToDelete)
    {
        var removeResult = test.RemoveQuestions(questionsToDelete);

        if (removeResult.IsFailure) return removeResult;

        foreach (var questionToAdd in questionsToAdd)
        {
            var addResult = test.AddQuestion(questionToAdd);
            await _questionsRepository.AddAsync(questionToAdd);
            if (addResult.IsFailure) return addResult;
        }

        foreach(var questionToUpdate in questionsToUpdate)
        {
            var updateResult = test.UpdateQuestion(questionToUpdate);
            await _questionsRepository.UpdateAsync(questionToUpdate);
            if (updateResult.IsFailure) return updateResult;
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    private async Task EnsureUniqueJoinId(Models.Test test)
    {
        if (string.IsNullOrEmpty(test.JoinId))
            test.GenerateNewJoinId();

        while (await _repository.IsJoinIdExist(test.JoinId))
            test.GenerateNewJoinId();
    }
}
