#nullable enable

namespace IRibeiroForHireAPI.Infrastructure.ExternalServices;

/// <summary>
/// Representa a estrutura de requisição enviada para a API do Google Gemini.
/// </summary>
/// <param name="Contents">Lista de conteúdos/mensagens da conversa.</param>
/// <param name="GenerationConfig">Parâmetros de configuração para a geração do texto.</param>
public record GeminiRequest(GeminiContent[] Contents, GeminiConfig GenerationConfig);

/// <summary>
/// Define um bloco de conteúdo na conversa com a IA.
/// </summary>
/// <param name="Parts">Array de partes que compõem o conteúdo (texto, mídia, etc).</param>
public record GeminiContent(GeminiPart[] Parts);

/// <summary>
/// Representa uma parte individual do conteúdo, geralmente o texto da pergunta ou do contexto.
/// </summary>
/// <param name="Text">O texto propriamente dito.</param>
public record GeminiPart(string Text);

/// <summary>
/// Configurações de hiperparâmetros para o modelo de linguagem.
/// </summary>
/// <param name="Temperature">Controla a criatividade da resposta (0.0 a 1.0).</param>
/// <param name="MaxOutputTokens">Limite máximo de tokens na resposta gerada.</param>
public record GeminiConfig(double Temperature, int MaxOutputTokens);

// Responses
/// <summary>
/// Representa a resposta estruturada retornada pela API do Gemini.
/// </summary>
/// <param name="Candidates">Lista de possíveis respostas (candidatas) geradas pelo modelo.</param>
public record GeminiResponse(GeminiCandidate[] Candidates);

/// <summary>
/// Representa uma candidata de resposta individual.
/// </summary>
/// <param name="Content">O conteúdo textual da resposta.</param>
public record GeminiCandidate(GeminiResponseContent Content);

/// <summary>
/// O conteúdo interno de uma candidata de resposta.
/// </summary>
public record GeminiResponseContent(GeminiPart[] Parts);

/// <summary>
/// Métodos de extensão para facilitar a extração de dados da resposta do Gemini.
/// </summary>
public static class GeminiResponseExtensions
{

    /// <summary>
    /// Extrai o texto da primeira candidata de resposta de forma segura.
    /// </summary>
    /// <param name="response">O objeto de resposta da API.</param>
    /// <returns>O texto limpo da resposta ou uma string vazia caso ocorra falha na estrutura.</returns>    
    public static string GetText(this GeminiResponse? response)
    {
        return response?.Candidates?.FirstOrDefault()
            ?.Content?.Parts?.FirstOrDefault()
            ?.Text?.Trim() ?? string.Empty;
    }
}