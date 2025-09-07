using AiTesting.Domain.Common;
using AiTesting.Domain.Repositories;
using AiTesting.Infrastructure.Persistence.Repositories;
using AiTesting.Infrastructure.Persistence.Repositories.Concreate;
using AiTesting.Infrastructure.Persistence.Repositories.Contracts;
using AiTesting.Infrastructure.Services.Http;
using AiTesting.Infrastructure.Services.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace AiTesting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IGuestRepository, GuestRepository>();
        services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
        services.AddScoped<ITestRepository, TestRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }
}
