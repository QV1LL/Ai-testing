using AiTesting.Domain.Models;
using AiTesting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories.Concreate;

internal class QuestionRepository(DbContext dbContext) : GenericRepository<Question>(dbContext), IQuestionRepository
{
    public override Task UpdateAsync(Question entity, CancellationToken cancellationToken = default)
    {
        base.UpdateAsync(entity, cancellationToken);

        foreach (var option in entity.Options)
        {
            if (DbContext.Set<AnswerOption>().Any(o => o.Id == option.Id))
                DbContext.Entry(option).State = EntityState.Modified;
            else
                DbContext.Set<AnswerOption>().Add(option);
        }

        foreach (var correctAnswer in entity.CorrectAnswers)
            DbContext.Entry(correctAnswer).State = EntityState.Modified;

        return Task.CompletedTask;
    }
}
