namespace AiTesting.Domain.Common.Specifications.Test;

internal class TestWithFullQuestionsAndTestAttemptsSpecification : DefaultSpecification<Models.Test>
{
    public TestWithFullQuestionsAndTestAttemptsSpecification()
    {
        AddInclude("Questions.Options");
        AddInclude("Questions.CorrectAnswers");
        AddInclude("TestAttempts");
    }
}
