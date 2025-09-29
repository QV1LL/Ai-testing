using AiTesting.Domain.Models;
using AiTesting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AiTesting.Infrastructure.Persistence.Repositories.Concreate;

public class TestAttemptRepository(DbContext dbContext) : GenericRepository<TestAttempt>(dbContext), ITestAttemptRepository
{
    public async Task<IReadOnlyList<TestAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<TestAttempt>()
                              .Where(t => t.UserId == userId)
                              .AsNoTracking()
                              .ToListAsync(cancellationToken);
    }

    public override async Task AddAsync(TestAttempt entity, CancellationToken cancellationToken = default)
    {
        await base.AddAsync(entity, cancellationToken);

        foreach (var answer in entity.Answers)
        {
            DbContext.Set<AttemptAnswer>().Add(answer);

            foreach (var option in answer.SelectedOptions)
            {
                DbContext.Set<AnswerOption>().Attach(option);
            }
        }
    }


    public override async Task UpdateAsync(TestAttempt entity, CancellationToken cancellationToken = default)
    {
        await base.UpdateAsync(entity, cancellationToken);

        var answersIds = await DbContext.Set<AttemptAnswer>()
                                        .Select(a => a.Id)
                                        .ToListAsync(cancellationToken: cancellationToken);
        
        var optionsIds = await DbContext.Set<AnswerOption>()
                                        .Select(o => o.Id)
                                        .ToListAsync(cancellationToken: cancellationToken);

        foreach (var answer in entity.Answers)
        {
            if (answersIds.Contains(answer.Id))
                DbContext.Entry(answer).State = EntityState.Modified;
            else
                DbContext.Set<AttemptAnswer>().Add(answer);

            foreach (var option in answer.SelectedOptions)
            {
                if (optionsIds.Contains(option.Id))
                    DbContext.Entry(option).State = EntityState.Modified;
                else
                    DbContext.Set<AnswerOption>().Add(option);
            }
        }
    }

}
