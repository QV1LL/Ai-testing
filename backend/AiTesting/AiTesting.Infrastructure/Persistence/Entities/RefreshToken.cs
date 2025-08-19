using AiTesting.Domain.Models;
using System.Security.Cryptography;

namespace AiTesting.Infrastructure.Persistence.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    protected RefreshToken() { }

    private RefreshToken(User user, string token, DateTime expiresAt)
    {
        if (user.Id == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty");

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty");

        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("ExpiresAt must be in the future");

        Id = Guid.NewGuid();
        User = user;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(User user, int validDays = 7)
    {
        var token = GenerateToken();
        var expiresAt = DateTime.UtcNow.AddDays(validDays);
        return new RefreshToken(user, token, expiresAt);
    }

    public void Refresh(int validDays = 7)
    {
        Token = GenerateToken();
        ExpiresAt = DateTime.UtcNow.AddDays(validDays);
    }

    private static string GenerateToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
}
