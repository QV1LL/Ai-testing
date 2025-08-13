using AiTesting.Domain.Common;

namespace AiTesting.Domain.Models;

public class Test
{
    public Guid Id { get; private set; }

    public string Title
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty");
            field = value;
        }
    }

    public string Description
    {
        get => field;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Description cannot be empty");
            field = value;
        }
    }
    public string CoverImageUrl { get; private set; }
    public User CreatedBy { get; private set; }
    public Guid CreatedById { get; private set; }
    public bool IsPublic { get; private set; }
    public List<Question> Questions { get; private set; } = [];
    public List<TestAttempt> TestAttempts { get; private set; } = [];
    public int? TimeLimitMinutes { get; private set; }

    protected Test() { }

    private Test(Guid id, 
                 string title, 
                 string description, 
                 string coverImageUrl, 
                 User createdBy, 
                 bool isPublic, 
                 int? timeLimitMinutes)
    {
        Id = id;
        Title = title;
        Description = description;
        CoverImageUrl = coverImageUrl;
        CreatedBy = createdBy;
        IsPublic = isPublic;
        TimeLimitMinutes = timeLimitMinutes;
    }

    public static Result<Test> Create(string title, 
                                      string description, 
                                      User createdBy, 
                                      bool isPublic, 
                                      string coverImageUrl = null!, 
                                      int? timeLimitMinutes = null)
    {
        try
        {
            return Result<Test>.Success(
                new Test(Guid.NewGuid(), title, description, coverImageUrl, createdBy, isPublic, timeLimitMinutes)
                );
        }
        catch (ArgumentException ex)
        {
            return Result<Test>.Failure(ex.Message);
        }
    }

    public Result AddQuestion(Question question)
    {
        if (question == null) return Result.Failure("Question cannot be null");
        Questions.Add(question);
        return Result.Success();
    }
}

