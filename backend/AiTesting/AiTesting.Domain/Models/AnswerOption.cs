using AiTesting.Domain.Common;

namespace AiTesting.Domain.Models;

public class AnswerOption
{
    public Guid Id { get; private set; }
    public Question Question { get; private set; }
    public Guid QuestionId { get; private set; }

    public string Text
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Option text cannot be empty");
            field = value;
        }
    }

    public string ImageUrl { get; private set; }

    protected AnswerOption() { }

    private AnswerOption(Guid id, 
                         Question question, 
                         string text, 
                         string imageUrl)
    {
        Id = id;
        Question = question;
        Text = text;
        ImageUrl = imageUrl;
    }

    public static Result<AnswerOption> Create(Question question, 
                                              string text, 
                                              string imageUrl = null)
    {
        try
        {
            return Result<AnswerOption>.Success(
                new AnswerOption(Guid.NewGuid(), question, text, imageUrl)
                );
        }
        catch (ArgumentException ex)
        {
            return Result<AnswerOption>.Failure(ex.Message);
        }
    }
}
