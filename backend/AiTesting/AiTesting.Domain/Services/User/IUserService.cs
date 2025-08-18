using AiTesting.Domain.Common;

namespace AiTesting.Domain.Services.User;

public interface IUserService
{
    Task<Result<Models.User>> GetAsync(Guid id);
    Task<Result<Models.User>> GetByEmailAsync(string email);
    Task<Result> AddAsync(Models.User user);
    Task<Result> UpdateAsync(Models.User user);
    Task<Result> DeleteAsync(Models.User user);
}
