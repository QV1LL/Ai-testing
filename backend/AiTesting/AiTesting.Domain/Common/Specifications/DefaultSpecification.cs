namespace AiTesting.Domain.Common.Specifications;

public class DefaultSpecification<T> : ISpecification<T>
{
    public List<string> Includes { get; } = [];

    protected void AddInclude(string include)
    {
        Includes.Add(include);
    }
}
