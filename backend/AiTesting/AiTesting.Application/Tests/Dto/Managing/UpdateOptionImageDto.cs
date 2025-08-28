using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Tests.Dto.Managing;

public record UpdateOptionImageDto(
    Guid Id,
    Guid QuestionId,
    Guid TestId,
    IFormFile ImageFile
);
