// adaptador de saída
using IRibeiroForHire.Services.Interfaces;
using IRibeiroForHireAPI.Infrastructure.Configurations;
using IRibeiroForHireAPI.Infrastructure.ExternalServices.Responses;

namespace IRibeiroForHireAPI.Infrastructure.ExternalServices;

public class GeminiTextService : IGeminiTextService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;

    public GeminiTextService(HttpClient httpClient, GeminiOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<string> GenerateAnswerAsync(string context, string question)
    {
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{_options.Model}:generateContent?key={_options.ApiKey}";

        string formattedPrompt = _options.SystemPrompt
            .Replace("{context}", context)
            .Replace("{question}", question);

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = formattedPrompt } } } },
            generationConfig = new
            {
                temperature = _options.Temperature,
                maxOutputTokens = _options.MaxTokens
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
                return "Erro ao acessar inteligência artificial.";

            var data = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            return data.Candidates[0].Content.Parts[0].Text.Trim();
        }
        catch (Exception)
        {
            return "Erro técnico na comunicação com a IA.";
        }
    }
}