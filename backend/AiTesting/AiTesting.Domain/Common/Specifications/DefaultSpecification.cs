using System.Linq.Expressions;

namespace AiTesting.Domain.Common.Specifications;

public class DefaultSpecification<T> : ISpecification<T>
{
    public List<Expression<Func<T, object>>> Includes { get; } = [];

    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }
}
