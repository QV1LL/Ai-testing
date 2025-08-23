namespace AiTesting.Application.Tests.Dto.Managing;

public record TestDto(
    Guid Id,
    string Title,
    string? Description
)
{
}
