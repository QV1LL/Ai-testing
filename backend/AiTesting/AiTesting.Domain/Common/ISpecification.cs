namespace AiTesting.Domain.Common;

public interface ISpecification<T>
{
    List<string> Includes { get; }
}
