using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;

namespace AiTesting.Domain.Services.Guest;

internal class GuestService : IGuestService
{
    private readonly IGuestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public GuestService(IUnitOfWork unitOfWork, IGuestRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> AddAsync(Models.Guest guest)
    {
        await _repository.AddAsync(guest);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
