using AiTesting.Domain.Common;
using AiTesting.Domain.Models;
using AiTesting.Infrastructure.Services.Llm.Helpers;
using Google;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTesting.Infrastructure.Services.Llm;

public class GeminiLlmService : ILlmService
{
    private readonly Client _client;
    private readonly string _modelName;

    public GeminiLlmService(IConfiguration configuration)
    {
        Console.WriteLine(System.Environment.GetEnvironmentVariable("GOOGLE_AI_API"));
        
        var apiKey = System.Environment.GetEnvironmentVariable("GOOGLE_AI_API") ?? throw new InvalidOperationException("GOOGLE_AI_API environment variable is required.");
        _modelName = configuration["Llm:Gemini:ModelName"] ?? throw new InvalidOperationException("Gemini:ModelName configuration is required.");
        _client = new Client(apiKey: apiKey);
    }

    public async Task<Result<T>> GenerateUpdateQuestionsDto<T>(Test test, string prompt)
    {
        ArgumentNullException.ThrowIfNull(test);
        if (string.IsNullOrWhiteSpace(prompt)) return Result<T>.Failure("Prompt cannot be null or empty.");

        try
        {
            var serializeOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = false
            };

            var testJson = JsonSerializer.Serialize(test, serializeOptions);
            var fullPrompt = TemplateLoaderHelper.LoadGenerateTemplate()
                .Replace("{testJson}", testJson)
                .Replace("{userInstructions}", prompt);

            var contents = new List<Content>
            {
                new Content
                {
                    Role = "user",
                    Parts = [ new Part { Text = fullPrompt } ]
                }
            };

            var config = new GenerateContentConfig
            {
                MaxOutputTokens = 15000,
                Temperature = 0.2f,
                ResponseMimeType = "application/json"
            };

            var response = await _client.Models.GenerateContentAsync(_modelName, contents, config);

            if (response.Candidates == null || response.Candidates.Count == 0)
                return Result<T>.Failure("Invalid response from LLM: no candidates.");

            var candidate = response.Candidates[0];
            if (candidate.Content == null || candidate.Content.Parts == null || candidate.Content.Parts.Count == 0)
                return Result<T>.Failure("Invalid response from LLM: no content parts.");

            var jsonOutput = candidate.Content.Parts[0].Text?.Trim();

            if (string.IsNullOrWhiteSpace(jsonOutput))
                return Result<T>.Failure("Empty response from LLM.");

            jsonOutput = JsonHelper.NormalizeJsonString(jsonOutput);

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
        catch (GoogleApiException apiEx)
        {
            return Result<T>.Failure($"Gemini API request failed: {apiEx.Message}");
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

        var fullPrompt = TemplateLoaderHelper.LoadCheckAnswerTemplate()
            .Replace("{questionText}", questionText)
            .Replace("{correctAnswer}", correctAnswer)
            .Replace("{userAnswer}", userAnswer);

        var contents = new List<Content>
        {
            new Content
            {
                Role = "user",
                Parts = [ new Part { Text = fullPrompt } ]
            }
        };

        var config = new GenerateContentConfig
        {
            MaxOutputTokens = 200,
            Temperature = 0.1f,
            ResponseMimeType = "application/json"
        };

        var response = await _client.Models.GenerateContentAsync(_modelName, contents, config);

        if (response.Candidates == null || response.Candidates.Count == 0)
            throw new InvalidOperationException("Invalid response from LLM: no candidates.");

        var candidate = response.Candidates[0];
        if (candidate.Content == null || candidate.Content.Parts == null || candidate.Content.Parts.Count == 0)
            throw new InvalidOperationException("Invalid response from LLM: no content parts.");

        var jsonOutput = candidate.Content.Parts[0].Text?.Trim();

        if (string.IsNullOrWhiteSpace(jsonOutput))
            throw new InvalidOperationException("Empty response from LLM.");

        jsonOutput = JsonHelper.NormalizeJsonString(jsonOutput);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<JsonElement>(jsonOutput, options);

        if (!result.TryGetProperty("is_correct", out var isCorrectProp) ||
            !result.TryGetProperty("confidence", out var confidenceProp))
            throw new JsonException("Missing required fields in LLM response.");

        var isCorrect = isCorrectProp.GetBoolean();
        var confidence = confidenceProp.GetDouble();

        return isCorrect && confidence >= 0.8;
    }

    private async Task<string> FixJsonAsync(string invalidJson)
    {
        var repairPrompt = $@"
            Fix this JSON so that it becomes valid and ready for deserialization.
            Remove all unnecessary text, comments, quotes, or escape characters.
            Return only a valid, minified JSON without any explanations.

            JSON:
            {invalidJson}";

        var contents = new List<Content>
        {
            new Content
            {
                Role = "user",
                Parts = [ new Part { Text = repairPrompt } ]
            }
        };

        var config = new GenerateContentConfig
        {
            MaxOutputTokens = 8000,
            Temperature = 0.0f,
            ResponseMimeType = "application/json"
        };

        var response = await _client.Models.GenerateContentAsync(_modelName, contents, config);

        if (response.Candidates == null || response.Candidates.Count == 0)
            throw new InvalidOperationException("Invalid response from LLM while fixing JSON.");

        var candidate = response.Candidates[0];
        if (candidate.Content == null || candidate.Content.Parts == null || candidate.Content.Parts.Count == 0)
            throw new InvalidOperationException("Invalid response from LLM while fixing JSON: no content parts.");

        var fixedJson = candidate.Content.Parts[0].Text?.Trim();

        if (string.IsNullOrWhiteSpace(fixedJson))
            throw new InvalidOperationException("LLM returned empty JSON fix response.");

        return JsonHelper.NormalizeJsonString(fixedJson);
    }
}