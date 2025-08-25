namespace AiTesting.Domain.Common.Specifications.User;

public class UserWithTestsAndTestAttemptsSpecification : DefaultSpecification<Models.User>
{
    public UserWithTestsAndTestAttemptsSpecification()
    {
        AddInclude("Tests");
        AddInclude("TestAttempts");
    }
}
