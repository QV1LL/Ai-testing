namespace AiTesting.Domain.Common.Specifications.TestAttempt;

internal class TestAttemptWithGuest : DefaultSpecification<Models.TestAttempt>
{
    public TestAttemptWithGuest()
    {
        AddInclude("Guest");
    }
}
