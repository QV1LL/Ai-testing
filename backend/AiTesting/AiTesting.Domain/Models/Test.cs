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

    public string? Description { get; set; }
    public string CoverImageUrl { get; set; }
    public User CreatedBy { get; private set; }
    public Guid CreatedById { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsPublic { get; set; }
    public List<Question> Questions { get; private set; } = [];
    public List<TestAttempt> TestAttempts { get; private set; } = [];
    public int? TimeLimitMinutes
    {
        get => field;
        set
        {
            if (value == null)
                return;

            if (value <= 0)
                throw new ArgumentException("Time limit must be greater than 0 minutes");

            field = value;
        }
    }

    protected Test() { }

    private Test(Guid id, 
                 string title, 
                 string? description, 
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
        CreatedAt = DateTime.Now;
        IsPublic = isPublic;
        TimeLimitMinutes = timeLimitMinutes;
    }

    public static Result<Test> Create(string title, 
                                      string? description, 
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

    public Result RemoveQuestions(IEnumerable<Guid> questionIds)
    {
        if (questionIds == null)
            return Result.Failure("No question ids provided");

        Questions.RemoveAll(q => questionIds.Contains(q.Id));
        return Result.Success();
    }

    public Result UpdateQuestion(Question updatedQuestion)
    {
        var question = Questions.FirstOrDefault(q => q.Id == updatedQuestion.Id);
        if (question == null)
            return Result.Failure("Question not found");

        var updateMetadataResult = question.Update(
            updatedQuestion.Text,
            updatedQuestion.Order,
            updatedQuestion.ImageUrl,
            updatedQuestion.Type,
            updatedQuestion.CorrectTextAnswer
        );
        if (updateMetadataResult.IsFailure) return updateMetadataResult;

        var existingOptions = question.Options.ToDictionary(o => o.Id);
        var updatedOptions = updatedQuestion.Options.ToDictionary(o => o.Id);

        question.Options.RemoveAll(o => !updatedOptions.ContainsKey(o.Id));

        foreach (var option in updatedOptions.Values)
        {
            if (!existingOptions.ContainsKey(option.Id))
            {
                question.Options.Add(option);
            }
            else
            {
                var existing = existingOptions[option.Id];
                existing.Text = option.Text;
                existing.ImageUrl = option.ImageUrl;
            }
        }

        var existingCorrect = question.CorrectAnswers.ToDictionary(ca => ca.Id);
        var updatedCorrect = updatedQuestion.CorrectAnswers.ToDictionary(ca => ca.Id);

        question.CorrectAnswers.RemoveAll(ca => !updatedCorrect.ContainsKey(ca.Id));
        question.CorrectAnswers.AddRange(updatedCorrect.Values.Where(ca => !existingCorrect.ContainsKey(ca.Id)));
        question.SetCorrectTextAnswer(updatedQuestion.CorrectTextAnswer);

        return Result.Success();
    }
}

