namespace AiTesting.Application.Tests.Dto.Managing;

public record CreateTestResultDto
(
    Guid Id,
    string Title,
    string Description
)
{
}
