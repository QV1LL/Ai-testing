using AiTesting.Domain.Models;
using AiTesting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories.Concreate;

public class UserRepository(DbContext dbContext) : GenericRepository<User>(dbContext), IUserRepository
{

}
