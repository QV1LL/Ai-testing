using AiTesting.Domain.Models;
using AiTesting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories.Concreate;

internal class QuestionRepository(DbContext dbContext) : GenericRepository<Question>(dbContext), IQuestionRepository
{
    public override Task UpdateAsync(Question entity, CancellationToken cancellationToken = default)
    {
        base.UpdateAsync(entity, cancellationToken);

        var optionsIds = DbContext.Set<AnswerOption>().Select(c => c.Id).ToList();

        foreach (var option in entity.Options)
        {
            if (optionsIds.Contains(option.Id))
                DbContext.Entry(option).State = EntityState.Modified;
            else
                DbContext.Set<AnswerOption>().Add(option);
        }

        foreach (var correctAnswer in entity.CorrectAnswers)
        {
            if (optionsIds.Contains(correctAnswer.Id))
                DbContext.Entry(correctAnswer).State = EntityState.Modified;
            else
                DbContext.Set<AnswerOption>().Add(correctAnswer);
        }

        return Task.CompletedTask;
    }
}
