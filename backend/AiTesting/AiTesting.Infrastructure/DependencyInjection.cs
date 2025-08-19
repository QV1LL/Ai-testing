using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;
using AiTesting.Infrastructure.Persistence.Repositories;
using AiTesting.Infrastructure.Persistence.Repositories.Concreate;
using AiTesting.Infrastructure.Persistence.Repositories.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace AiTesting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IGuestRepository, GuestRepository>();
        services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
        services.AddScoped<ITestRepository, TestRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
