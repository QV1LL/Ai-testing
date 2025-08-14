namespace AiTesting.Domain.Common;

public interface ISpecification<T>
{
    IQueryable<T> Apply(IQueryable<T> query);
}
