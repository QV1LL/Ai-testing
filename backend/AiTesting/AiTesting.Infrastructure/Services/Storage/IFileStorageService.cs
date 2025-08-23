using AiTesting.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace AiTesting.Infrastructure.Services.Storage;

public interface IFileStorageService
{
    public Task<Result<string>> UploadFileAsync(IFormFile? file, string subfolder);
    public Task<Result> DeleteFileAsync(string fileName);
}
