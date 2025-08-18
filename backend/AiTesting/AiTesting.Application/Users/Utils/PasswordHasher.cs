using Microsoft.AspNetCore.Identity;

namespace AiTesting.Application.Users.Utils;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password) => _hasher.HashPassword(null!, password);
    public bool VerifyPassword(string hashedPassword, string password) =>
        _hasher.VerifyHashedPassword(null!, hashedPassword, password) == PasswordVerificationResult.Success;
}
