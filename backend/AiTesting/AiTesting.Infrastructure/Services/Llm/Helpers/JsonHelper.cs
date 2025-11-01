using System.Text.Json;
using System.Text.RegularExpressions;

namespace AiTesting.Infrastructure.Services.Llm.Helpers;

internal static class JsonHelper
{
    public static string NormalizeJsonString(string? jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            throw new InvalidOperationException("Empty or null JSON string received from LLM.");

        jsonString = ExtractJsonBody(jsonString);
        jsonString = CleanFormatting(jsonString);
        jsonString = EscapeInnerQuotes(jsonString);
        jsonString = FixDoubleEscaping(jsonString);

        ValidateJson(jsonString);

        return jsonString;
    }

    private static string ExtractJsonBody(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        if (start >= 0 && end > start)
            text = text.Substring(start, end - start + 1);

        return text;
    }

    private static string CleanFormatting(string json)
    {
        return json
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\t", "")
            .Trim();
    }

    private static string EscapeInnerQuotes(string json)
    {
        return Regex.Replace(json, @":\s*""((?:[^""\\]|\\.)*)""", match =>
        {
            string value = match.Groups[1].Value;
            string escaped = value.Replace("\"", "\\\"");
            return $": \"{escaped}\"";
        });
    }

    private static string FixDoubleEscaping(string json)
    {
        return json.Replace("\\\\\"", "\\\"");
    }

    private static void ValidateJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Failed to parse corrected JSON: {ex.Message}\nJSON: {json}"
            );
        }
    }
}
