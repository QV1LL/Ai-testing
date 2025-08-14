using AiTesting.Domain.Common;

namespace AiTesting.Domain.Models;

public class Guest : IUser
{
    public Guid Id { get; private set; }

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

    protected Guest() { }

    private Guest(Guid id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    public static Result<Guest> Create(string displayName)
    {
        try
        {
            return Result<Guest>.Success(
                new Guest(Guid.NewGuid(), displayName)
                );
        }
        catch(ArgumentException ex)
        {
            return Result<Guest>.Failure(ex.Message);
        }
    }
}
