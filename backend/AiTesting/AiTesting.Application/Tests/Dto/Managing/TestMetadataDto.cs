namespace AiTesting.Application.Tests.Dto.Managing;

public record TestMetadataDto(
    Guid Id,
    string Title,
    string? Description
);
