using AiTesting.Domain.Common;
using AiTesting.Domain.Enums;
using System.Text.RegularExpressions;

namespace AiTesting.Domain.Models;

public class User : IUser
{
    public Guid Id { get; private set; }

    public string Email
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty");

            if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Invalid email format");

            field = value;
        }
    }

    public string PasswordHash
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Password hash cannot be empty");

            field = value;
        }
    }

    public string DisplayName
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Display name cannot be empty");

            if (value.Length < 3 || value.Length > 50)
                throw new ArgumentException("Display name must be between 3 and 50 characters");

            field = value;
        }
    }

    public UserRole Role { get; private set; }
    public List<Test> Tests { get; private set; } = [];
    public List<TestAttempt> TestAttempts { get; private set; } = [];

    protected User() { }

    private User(Guid id, 
                string email, 
                string passwordHash, 
                string displayName, 
                UserRole role)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        DisplayName = displayName;
        Role = role;
    }

    public static Result<User> Create(string email, 
                              string passwordHash, 
                              string displayName, 
                              UserRole role = UserRole.Default)
    {
        try
        {
            return Result<User>.Success(
                new User(Guid.NewGuid(), email, passwordHash, displayName, role)
                );
        }
        catch(ArgumentException ex)
        {
            return Result<User>.Failure(ex.Message);
        }
    }
}
