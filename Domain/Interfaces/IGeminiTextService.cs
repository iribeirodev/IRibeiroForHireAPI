namespace IRibeiroForHire.Services.Interfaces;

/// <summary>
/// Domínio para serviços de IA Generativa
/// </summary>
public interface IGeminiTextService
{
    /// <summary>
    /// Gera uma resposta baseada em um contexto técnico e uma pergunta do usuário
    /// </summary>
    /// <param name="context">Dados recuperados (ex: via Vector Search) para alimentar a IA</param>
    /// <param name="question">Dúvida enviada pelo usuário</param>
    /// <returns>Texto processado pela IA</returns>    
    Task<string> GenerateAnswerAsync(string context, string question);
}
