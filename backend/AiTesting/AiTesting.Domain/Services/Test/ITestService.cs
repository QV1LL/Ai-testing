using AiTesting.Domain.Common;
using AiTesting.Domain.Models;

namespace AiTesting.Domain.Services.Test;

public interface ITestService
{
    Task<Result<Models.Test>> GetByIdAsync(Guid id);
    Task<Result<Models.Test>> GetByJoinIdAsync(string id);
    Task<Result<Models.Test>> GetMetadataByJoinIdAsync(string id);
    Task<Result> AddAsync(Models.Test test);
    Task<Result> UpdateAsync(Models.Test test);
    Task<Result> UpdateTestQuestionsAsync(
        Models.Test test,
        IEnumerable<Question> questionsToAdd,
        IEnumerable<Question> questionsToUpdate,
        IEnumerable<Guid> questionsToDelete);
    Task<Result> DeleteAsync(Guid id);
}
