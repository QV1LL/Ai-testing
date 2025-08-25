using AiTesting.Domain.Common;
using AiTesting.Domain.Enums;

namespace AiTesting.Domain.Models;

public class Question
{
    public Guid Id { get; private set; }
    public Test Test { get; private set; }
    public Guid TestId { get; private set; }
    public QuestionType Type { get; private set; }

    public string Text
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Question text cannot be empty");
            field = value;
        }
    }

    public string ImageUrl { get; private set; }
    public List<AnswerOption> Options { get; private set; } = [];
    public List<AnswerOption> CorrectAnswers { get; private set; } = [];
    public int Order { get; private set; }

    protected Question() { }

    private Question(Guid id, 
                     Test test, 
                     QuestionType type, 
                     string text, 
                     string imageUrl, 
                     int order)
    {
        Id = id;
        Test = test;
        Type = type;
        Text = text;
        ImageUrl = imageUrl;
        Order = order;
    }

    public static Result<Question> Create(Test test, 
                                          QuestionType type, 
                                          string text, 
                                          int order, 
                                          string imageUrl = "")
    {
        try
        {
            return Result<Question>.Success(
                new Question(Guid.NewGuid(), test, type, text, imageUrl, order)
                );
        }
        catch (ArgumentException ex)
        {
            return Result<Question>.Failure(ex.Message);
        }
    }

    public Result AddOption(AnswerOption option)
    {
        if (option == null) return Result.Failure("Option cannot be null");
        Options.Add(option);
        return Result.Success();
    }

    public Result SetCorrectAnswers(IEnumerable<AnswerOption> correctAnswers)
    {
        if (correctAnswers == null) return Result.Failure("Correct answers cannot be null");
        CorrectAnswers.AddRange(correctAnswers);
        return Result.Success();
    }
}
