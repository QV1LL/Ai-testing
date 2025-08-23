using AiTesting.Domain.Common;

namespace AiTesting.Domain.Models;

public class TestAttempt
{
    public Guid Id { get; private set; }
    public User? User { get; private set; }
    public Guid? UserId { get; private set; }
    public Guest? Guest { get; private set; }
    public Guid? GuestId { get; private set; }
    public Test Test { get; private set; }
    public Guid TestId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? FinishedAt { get; private set; }
    public double Score { get; private set; }
    public List<AttemptAnswer> Answers { get; private set; } = new();

    protected TestAttempt() { }

    private TestAttempt(Guid id, User? user, Guest? guest, Test test)
    {
        Id = id;
        User = user;
        Guest = guest;
        Test = test;
        StartedAt = DateTime.UtcNow;
    }

    public static Result<TestAttempt> Create(Test test, User? user = null, Guest? guest = null)
    {
        if ((user == null && guest == null) || (user != null && guest != null))
            return Result<TestAttempt>.Failure("Either user or guest must be provided, but not both.");

        return Result<TestAttempt>.Success(new TestAttempt(Guid.NewGuid(), user, guest, test));
    }

    public Result AddAnswer(AttemptAnswer answer)
    {
        if (answer == null) return Result.Failure("Answer cannot be null");
        Answers.Add(answer);
        return Result.Success();
    }

    public void Finish(double score)
    {
        Score = score;
        FinishedAt = DateTime.UtcNow;
    }
}
