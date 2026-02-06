using IRibeiroForHireAPI.Domain.Entities;

namespace IRibeiroForHireAPI.Domain.Interfaces;

/// <summary>
/// Serviço de aplicação responsável por orquestrar o fluxo de perguntas e respostas.
/// Implementa o padrão RAG (Retrieval-Augmented Generation) para fornecer respostas contextualizadas.
/// </summary>
/// <param name="repository">Repositório para persistência das interações no banco de dados.</param>
/// <param name="textGenerator">Serviço de integração com a API do Google Gemini.</param>
/// <param name="vectorSearch">Serviço de busca semântica e geração de embeddings (pgvector).</param>
public interface IQuestionService
{
    /// <summary>
    /// Processa uma nova pergunta através de um fluxo inteligente de três etapas: 
    /// 1. Geração de Embedding, 2. Busca de Contexto e 3. Geração de Resposta via IA.
    /// </summary>
    /// <remarks>
    /// O fluxo utiliza busca vetorial para encontrar informações no currículo/portfólio do Itamar 
    /// que sejam semanticamente similares à pergunta do usuário antes de enviar para a IA.
    /// </remarks>
    /// <param name="question">O texto da pergunta enviada pelo usuário.</param>
    /// <param name="visitorId">ID único do visitante para fins de histórico.</param>
    /// <param name="ip">Endereço IP do solicitante para auditoria e controle.</param>
    /// <returns>A resposta gerada pela IA baseada no contexto encontrado.</returns>
    Task<string> AskQuestionAsync(
        string question, 
        string visitorId, 
        string ip);

    /// <summary>
    /// Recupera o histórico de todas as interações realizadas por um visitante específico.
    /// </summary>
    /// <param name="visitorId">Identificador único (hash) do visitante.</param>
    /// <returns>Uma lista de interações contendo perguntas, respostas e metadados.</returns>
    Task<List<QaInteraction>> GetInteractions(string visitorId);

    /// <summary>
    /// Verifica se o database pode ser conectado.
    /// </summary>
    Task<bool> CheckDatabaseConnection();
}