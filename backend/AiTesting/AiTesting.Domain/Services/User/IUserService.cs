using AiTesting.Domain.Common;

namespace AiTesting.Domain.Services.User;

public interface IUserService
{
    Task<Result<Models.User>> Get(Guid id);
    Task<Result> Add(Models.User user);
    Task<Result> Update(Models.User user);
    Task<Result> Delete(Models.User user);
}
