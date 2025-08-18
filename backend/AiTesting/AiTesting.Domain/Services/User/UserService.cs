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

    public async Task<Result> AddAsync(Models.User user)
    {
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Models.User user)
    {
        await _userRepository.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<Models.User>> GetAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return user == null ? 
               Result<Models.User>.Failure("User not found") : 
               Result<Models.User>.Success(user);
    }

    public async Task<Result<Models.User>> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        return user == null ?
               Result<Models.User>.Failure("User not found") :
               Result<Models.User>.Success(user);
    }

    public async Task<Result> UpdateAsync(Models.User user)
    {
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
