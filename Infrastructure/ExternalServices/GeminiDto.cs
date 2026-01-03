#nullable enable

namespace IRibeiroForHireAPI.Infrastructure.ExternalServices;

// Requests
public record GeminiRequest(GeminiContent[] Contents, GeminiConfig GenerationConfig);
public record GeminiContent(GeminiPart[] Parts);
public record GeminiPart(string Text);
public record GeminiConfig(double Temperature, int MaxOutputTokens);

// Responses
public record GeminiResponse(GeminiCandidate[] Candidates);
public record GeminiCandidate(GeminiResponseContent Content);
public record GeminiResponseContent(GeminiPart[] Parts);


public static class GeminiResponseExtensions
{
    public static string GetText(this GeminiResponse? response)
    {
        return response?.Candidates?.FirstOrDefault()
            ?.Content?.Parts?.FirstOrDefault()
            ?.Text?.Trim() ?? string.Empty;
    }
}