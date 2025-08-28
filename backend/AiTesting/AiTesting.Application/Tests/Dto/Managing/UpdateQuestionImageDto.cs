using Microsoft.AspNetCore.Http;

namespace AiTesting.Application.Tests.Dto.Managing;

public record UpdateQuestionImageDto(
    Guid Id,
    Guid TestId,
    IFormFile ImageFile
);
