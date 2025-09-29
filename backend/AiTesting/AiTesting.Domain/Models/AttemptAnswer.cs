using AiTesting.Domain.Common;

namespace AiTesting.Domain.Models;

public class AttemptAnswer
{
    public Guid Id { get; private set; }
    public TestAttempt Attempt { get; private set; }
    public Guid AttemptId { get; private set; }
    public Question Question { get; private set; }
    public Guid QuestionId { get; private set; }
    public List<AnswerOption> SelectedOptions { get; private set; } = [];
    public string? WrittenAnswer { get; private set; }

    protected AttemptAnswer() { }

    private AttemptAnswer(Guid id, 
                          TestAttempt attempt, 
                          Question question, 
                          List<AnswerOption> selectedOptions = null!, 
                          string? writtenAnswer = null!)
    {
        Id = id;
        Attempt = attempt;
        Question = question;
        SelectedOptions = selectedOptions ?? [];
        WrittenAnswer = writtenAnswer;
    }

    public static Result<AttemptAnswer> Create(TestAttempt attempt, 
                                               Question question, 
                                               List<AnswerOption> selectedOptions = null!, 
                                               string? writtenAnswer = null!)
    {
        return Result<AttemptAnswer>.Success(
            new AttemptAnswer(Guid.NewGuid(), attempt, question, selectedOptions, writtenAnswer)
            );
    }
}
