namespace AiTesting.Domain.Common.Specifications.Test;

internal class TestWithQuestionsSpecification : DefaultSpecification<Models.Test>
{
    public TestWithQuestionsSpecification()
    {
        AddInclude("Questions.Options");
        AddInclude("Questions.CorrectAnswers");
    }
}
