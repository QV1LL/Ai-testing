using System.Linq.Expressions;

namespace AiTesting.Domain.Common;

public interface ISpecification<T>
{
    List<Expression<Func<T, object>>> Includes { get; }
}
