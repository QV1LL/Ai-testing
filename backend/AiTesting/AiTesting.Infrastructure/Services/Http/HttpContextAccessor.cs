namespace AiTesting.Infrastructure.Services.Http;

internal class HttpContextAccessor : IHttpContextAccessor
{
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _contextAccessor;

    public HttpContextAccessor(Microsoft.AspNetCore.Http.IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public string GetApiUrl()
    {
        var request = _contextAccessor.HttpContext?.Request;

        if (request == null)
            throw new InvalidOperationException("HttpContext is not available");

        return $"{request.Scheme}://{request.Host}";
    }
}
