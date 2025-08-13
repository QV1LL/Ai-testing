namespace AiTesting.Domain.Models;

public interface IUser
{
    public Guid Id { get; }
    public string DisplayName { get; set; }
}
