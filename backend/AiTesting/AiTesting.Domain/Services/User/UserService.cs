using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;

namespace AiTesting.Domain.Services.User;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _userRepository = unitOfWork.Users;
    }

    public async Task<Result> Add(Models.User user)
    {
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> Delete(Models.User user)
    {
        await _userRepository.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Models.User>> Get(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return user == null ? 
               Result<Models.User>.Failure("User not found") : 
               Result<Models.User>.Success(user);
    }

    public async Task<Result> Update(Models.User user)
    {
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
