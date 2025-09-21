using AiTesting.Domain.Common;

namespace AiTesting.Domain.Services.Guest;

public interface IGuestService
{
    Task<Result> AddAsync(Models.Guest guest);
}
