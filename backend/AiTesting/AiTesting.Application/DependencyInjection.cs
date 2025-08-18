using AiTesting.Application.Users.Services.Auth;
using AiTesting.Application.Users.Services.Profile;
using AiTesting.Application.Users.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AiTesting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserProfileService, UserProfileService>();

        return services;
    }
}
