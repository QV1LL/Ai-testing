using AiTesting.Application.Tests.Services.Managing;
using AiTesting.Application.Users.Services.Auth;
using AiTesting.Application.Users.Services.Profile;
using AiTesting.Application.Users.Services.RefreshToken;
using AiTesting.Application.Users.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AiTesting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserProfileService, UserProfileService>();

        services.AddScoped<ITestManageService, TestManageService>();

        return services;
    }
}
