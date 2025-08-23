using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;

namespace AiTesting.Domain.Services.Test;

internal class TestService : ITestService
{
    private readonly ITestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TestService(ITestRepository repository, 
                       IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> AddAsync(Models.Test test)
    {
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
}
