using IRibeiroForHire.Services.Interfaces;
using IRibeiroForHireAPI.Infrastructure.Configurations;

namespace IRibeiroForHireAPI.Infrastructure.ExternalServices;

public class GeminiTextService(HttpClient httpClient, GeminiOptions options) : IGeminiTextService
{
    public async Task<string> GenerateAnswerAsync(
        string context,
        string question)
    {
        string baseUrl = options.BaseUrl.EndsWith("/") ? options.BaseUrl : options.BaseUrl + "/";
        string url = $"{baseUrl}models/{options.Model}:generateContent?key={options.ApiKey}";
        
        string formattedPrompt = options.SystemPrompt
            .Replace("{context}", context)
            .Replace("{question}", question);

        var requestBody = new
        {
            contents = new[]
            {
            new
            {
                parts = new[]
                {
                    new { text = formattedPrompt }
                }
            }
        },
            generationConfig = new
            {
                temperature = options.Temperature,
                maxOutputTokens = options.MaxTokens
            }
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync(url, requestBody);
            var rawResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("------- ERRO API GEMINI -------");
                System.Diagnostics.Debug.WriteLine(rawResponse);
                return "No momento, estou com dificuldade de acessar as informações. Por favor, tente novamente em instantes.";
            }

            Console.WriteLine($"Tamanho da resposta: {rawResponse.Length}");
            using var doc = System.Text.Json.JsonDocument.Parse(rawResponse);

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                return "Desculpe, não consegui elaborar uma resposta sobre isso agora.";
            }

            var firstCandidate = candidates[0];

            if (firstCandidate.TryGetProperty("content", out var content) &&
                content.TryGetProperty("parts", out var parts) &&
                parts.GetArrayLength() > 0)
            {
                string aiText = parts[0].GetProperty("text").GetString() ?? "";
                return aiText.Trim();
            }

            return "O Itamar tem um perfil interessante, mas não consegui processar essa pergunta específica.";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ERRO TÉCNICO]: {ex.Message}");
            return "Ocorreu um erro técnico na minha central de inteligência.";
        }
    }
    #region Private Methods
    private string FormatPrompt(string context, string question) =>
        options.SystemPrompt
            .Replace("{context}", context)
            .Replace("{question}", question);
    #endregion
}