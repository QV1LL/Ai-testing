using AiTesting.Domain.Common;

namespace AiTesting.Domain.Services.Test;

public interface ITestService
{
    Task<Result<Models.Test>> GetByIdAsync(Guid id);
    Task<Result> AddAsync(Models.Test test);
    Task<Result> UpdateAsync(Models.Test test);
    Task<Result> DeleteAsync(Guid id);
}
