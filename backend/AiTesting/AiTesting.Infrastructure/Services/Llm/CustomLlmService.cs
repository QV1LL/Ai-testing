using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTesting.Infrastructure.Services.Llm;

public class CustomLlmService : ILlmService
{
    private readonly HttpClient _httpClient;
    private readonly string _modelName;
    private readonly Uri _llmEndPoint;

    private string _generateQuestionsTemplate;
    private string _checkAnswerTemplate;

    private const string GENERATE_QUESTIONS_TEMPLATE_PATH = "generate_questions_prompt.txt";
    private const string CHECK_ANSWERS_TEMPLATE_PATH = "check_answer_prompt.txt";

    public CustomLlmService(IConfiguration configuration)
    {
        _modelName = configuration["Llm:ModelName"] ?? string.Empty;
        var endPointString = configuration["Llm:EndPoint"] ?? string.Empty;
        _llmEndPoint = new Uri(endPointString);
        _httpClient = new HttpClient();
    }

    private void LoadGenerateTemplateIfNeeded()
    {
        if (string.IsNullOrWhiteSpace(_generateQuestionsTemplate))
        {
            var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GENERATE_QUESTIONS_TEMPLATE_PATH);
            if (!File.Exists(templateFilePath))
                throw new FileNotFoundException("Prompt template file not found.", templateFilePath);
            _generateQuestionsTemplate = File.ReadAllText(templateFilePath);
        }
    }

    private void LoadCheckTemplateIfNeeded()
    {
        if (string.IsNullOrWhiteSpace(_checkAnswerTemplate))
        {
            var templateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CHECK_ANSWERS_TEMPLATE_PATH);
            if (!File.Exists(templateFilePath))
                throw new FileNotFoundException("Prompt template file not found.", templateFilePath);
            _checkAnswerTemplate = File.ReadAllText(templateFilePath);
        }
    }

    public async Task<Result<T>> GenerateUpdateQuestionsDto<T>(Test test, string prompt)
    {
        ArgumentNullException.ThrowIfNull(test);
        if (string.IsNullOrWhiteSpace(prompt)) return Result<T>.Failure("Prompt cannot be null or empty.");
        if (string.IsNullOrWhiteSpace(_modelName)) return Result<T>.Failure("Model name is required.");

        LoadGenerateTemplateIfNeeded();

        try
        {
            var serializeOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = false
            };

            var testJson = JsonSerializer.Serialize(test, serializeOptions);
            var fullPrompt = _generateQuestionsTemplate
                .Replace("{testJson}", testJson)
                .Replace("{userInstructions}", prompt);

            var requestBody = new
            {
                model = _modelName,
                messages = new[] { new { role = "user", content = fullPrompt } },
                max_tokens = 1500,
                temperature = 0.2
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(new Uri(_llmEndPoint, "/v1/chat/completions"), content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
                return Result<T>.Failure("Invalid response from LLM: no choices.");

            var message = choices[0].GetProperty("message");
            var jsonOutput = message.GetProperty("content").GetString()?.Trim();

            if (string.IsNullOrWhiteSpace(jsonOutput))
                return Result<T>.Failure("Empty response from LLM.");

            jsonOutput = ExtractJsonFromResponse(jsonOutput);

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultValue = JsonSerializer.Deserialize<T>(jsonOutput, options);

                if (resultValue == null)
                    throw new JsonException("Deserialized object is null.");

                return Result<T>.Success(resultValue);
            }
            catch (JsonException)
            {
                var fixedJson = await FixJsonAsync(jsonOutput);
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var fixedResult = JsonSerializer.Deserialize<T>(fixedJson, options);
                    if (fixedResult == null)
                        return Result<T>.Failure("Failed to deserialize even after JSON correction.");

                    return Result<T>.Success(fixedResult);
                }
                catch (Exception innerEx)
                {
                    return Result<T>.Failure($"Failed to parse corrected JSON: {innerEx.Message}");
                }
            }
        }
        catch (HttpRequestException httpEx)
        {
            return Result<T>.Failure($"HTTP request failed: {httpEx.Message}");
        }
        catch (JsonException jsonEx)
        {
            return Result<T>.Failure($"JSON parsing error: {jsonEx.Message}");
        }
        catch (Exception ex)
        {
            return Result<T>.Failure($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<bool> IsCorrect(string userAnswer, string correctAnswer, string questionText)
    {
        if (string.IsNullOrWhiteSpace(userAnswer)) throw new ArgumentNullException(nameof(userAnswer));
        if (string.IsNullOrWhiteSpace(correctAnswer)) throw new ArgumentNullException(nameof(correctAnswer));
        if (string.IsNullOrWhiteSpace(questionText)) throw new ArgumentNullException(nameof(questionText));
        if (string.IsNullOrWhiteSpace(_modelName)) throw new ArgumentException("Model name is required.", nameof(_modelName));

        LoadCheckTemplateIfNeeded();

        var fullPrompt = _checkAnswerTemplate
            .Replace("{questionText}", questionText)
            .Replace("{correctAnswer}", correctAnswer)
            .Replace("{userAnswer}", userAnswer);

        var requestBody = new
        {
            model = _modelName,
            messages = new[] { new { role = "user", content = fullPrompt } },
            max_tokens = 200,
            temperature = 0.1
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(new Uri(_llmEndPoint, "/v1/chat/completions"), content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            throw new InvalidOperationException("Invalid response from LLM: no choices.");

        var message = choices[0].GetProperty("message");
        var jsonOutput = message.GetProperty("content").GetString()?.Trim();

        if (string.IsNullOrWhiteSpace(jsonOutput))
            throw new InvalidOperationException("Empty response from LLM.");

        jsonOutput = ExtractJsonFromResponse(jsonOutput);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<JsonElement>(jsonOutput, options);

        if (!result.TryGetProperty("is_correct", out var isCorrectProp) ||
            !result.TryGetProperty("confidence", out var confidenceProp))
            throw new JsonException("Missing required fields in LLM response.");

        var isCorrect = isCorrectProp.GetBoolean();
        var confidence = confidenceProp.GetDouble();

        return isCorrect && confidence >= 0.8;
    }

    private static string ExtractJsonFromResponse(string response)
    {
        if (response.StartsWith('{') && response.EndsWith('}'))
            return response;

        var start = response.IndexOf('{');
        var end = response.LastIndexOf('}');
        if (start >= 0 && end > start)
            return response.Substring(start, end - start + 1);

        return response;
    }

    private async Task<string> FixJsonAsync(string invalidJson)
    {
        var repairPrompt = $@"
            Fix this JSON so that it becomes valid and ready for deserialization.
            Remove all unnecessary text, comments, quotes, or escape characters.
            Return only a valid, minified JSON without any explanations.

            JSON:
            {invalidJson}";

        var requestBody = new
        {
            model = _modelName,
            messages = new[] { new { role = "user", content = repairPrompt } },
            max_tokens = 800,
            temperature = 0.0
        };

        var jsonRequest = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(new Uri(_llmEndPoint, "/v1/chat/completions"), content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        if (!doc.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            throw new InvalidOperationException("Invalid response from LLM while fixing JSON.");

        var message = choices[0].GetProperty("message");
        var fixedJson = message.GetProperty("content").GetString()?.Trim();

        if (string.IsNullOrWhiteSpace(fixedJson))
            throw new InvalidOperationException("LLM returned empty JSON fix response.");

        return ExtractJsonFromResponse(fixedJson);
    }
}
