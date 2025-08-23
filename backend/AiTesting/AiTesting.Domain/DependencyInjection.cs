using AiTesting.Domain.Services.Test;
using AiTesting.Domain.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace AiTesting.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITestService, TestService>();

        return services;
    }
}
