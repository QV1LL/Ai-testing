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

    public string ImageUrl { get; set; }
    public List<AnswerOption> Options { get; private set; } = [];
    public List<AnswerOption> CorrectAnswers { get; private set; } = [];
    public string? CorrectTextAnswer { get; private set; }
    public int Order { get; private set; }

    protected Question() { }

    private Question(Guid id, 
                     Test test, 
                     QuestionType type, 
                     string text, 
                     string imageUrl,
                     string? correctTextAnswer,
                     int order)
    {
        Id = id;
        Test = test;
        Type = type;
        Text = text;
        ImageUrl = imageUrl;
        CorrectTextAnswer = correctTextAnswer;
        Order = order;
    }

    public static Result<Question> Create(Test test, 
                                          QuestionType type, 
                                          string text, 
                                          int order, 
                                          string? correctTextAnswer = null,
                                          string imageUrl = "")
    {
        try
        {
            return Result<Question>.Success(
                new Question(Guid.NewGuid(), test, type, text, imageUrl, correctTextAnswer, order)
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
        if (Type == QuestionType.OpenEnded)
            return Result.Failure("Open answer questions cannot have option-based correct answers");

        if (correctAnswers == null || !correctAnswers.Any())
            return Result.Failure("Correct answers cannot be empty");

        var invalidAnswers = correctAnswers
            .Where(a => !Options.Any(o => o.Id == a.Id))
            .ToList();

        if (invalidAnswers.Count != 0)
            return Result.Failure("Some correct answers are not part of the question options");

        CorrectAnswers.Clear();
        CorrectAnswers.AddRange(correctAnswers);

        return Result.Success();
    }

    public Result SetCorrectTextAnswer(string? correctAnswer)
    {
        if (Type != QuestionType.OpenEnded)
            return Result.Failure("Only open answer questions can have a text answer");

        CorrectTextAnswer = correctAnswer;
        return Result.Success();
    }

    public Result Update(
        string text,
        int order,
        string imageUrl,
        QuestionType type,
        string? correctTextAnswer = null)
    {
        try
        {
            Text = text;
            CorrectTextAnswer = correctTextAnswer;
            Order = order;
            ImageUrl = imageUrl;
            Type = type;

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
