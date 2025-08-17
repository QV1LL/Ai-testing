using AiTesting.Application.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AiTesting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IPasswordHasher, PasswordHasher>();

        return services;
    }
}
