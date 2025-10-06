using AiTesting.Domain.Common;
using AiTesting.Domain.Models;

namespace AiTesting.Domain.Repositories;

public interface ITestRepository : IRepository<Test>
{
    Task<Test?> GetByJoinIdAsync(string joinId, ISpecification<Test> specification, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsJoinIdExist(string joinId);
}
