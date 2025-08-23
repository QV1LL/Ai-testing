using AiTesting.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace AiTesting.Infrastructure.Services.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    private const string UPLOADS_SUBDIRECTORY = "uploads";

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<Result<string>> UploadFileAsync(IFormFile? file, string subfolder)
    {
        if (file == null || file.Length == 0)
            return Result<string>.Failure("File is empty");

        try
        {
            var uploadsPath = Path.Combine(_env.ContentRootPath, UPLOADS_SUBDIRECTORY, subfolder);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/{UPLOADS_SUBDIRECTORY}/{subfolder}/{fileName}";
            return Result<string>.Success(url);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Failed to upload file: {ex.Message}");
        }
    }

    public Task<Result> DeleteFileAsync(string fileName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Task.FromResult(Result.Failure("File name is empty"));

            var filePath = Path.Combine(_env.ContentRootPath, fileName.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(filePath))
                File.Delete(filePath);

            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Failure($"Failed to delete file: {ex.Message}"));
        }
    }
}
