using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Tests.Dto.Managing;

public record UpdateTestMetadataDto(
    Guid Id,
    string Title,
    string? Description,
    int? TimeLimitInMinutes,
    bool IsPublic,
    IFormFile? CoverImage
);
