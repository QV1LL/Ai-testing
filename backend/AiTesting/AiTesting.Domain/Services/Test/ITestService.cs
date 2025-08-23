using AiTesting.Domain.Common;

namespace AiTesting.Domain.Services.Test;

public interface ITestService
{
    Task<Result> AddAsync(Models.Test test);
    Task<Result> UpdateAsync(Models.Test test);
    Task<Result> DeleteAsync(Guid id);
}
