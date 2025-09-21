using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;

namespace AiTesting.Domain.Services.TestAttempt;

internal class TestAttemptService : ITestAttemptService
{
    private readonly ITestAttemptRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TestAttemptService(IUnitOfWork unitOfWork, ITestAttemptRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> AddAsync(Models.TestAttempt testAttempt)
    {
        await _repository.AddAsync(testAttempt);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
