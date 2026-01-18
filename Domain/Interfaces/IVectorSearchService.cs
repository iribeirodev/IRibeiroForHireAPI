namespace IRibeiroForHire.Services.Interfaces;

/// <summary>
/// Serviço especializado em operações vetoriais e busca semântica (RAG).
/// Realiza a ponte entre a API de Embeddings do Google e o banco de dados PostgreSQL com extensão pgvector.
/// </summary>
/// <param name="httpClient">Cliente HTTP para comunicação com serviços externos de IA.</param>
/// <param name="options">Configurações globais da API, incluindo chaves de acesso e strings de conexão.</param>
public interface IVectorSearchService
{
/// <summary>
    /// Converte um texto plano em um vetor numérico (embedding) de alta dimensionalidade.
    /// Utiliza o modelo 'text-embedding-004' para gerar representações semânticas de 768 dimensões.
    /// </summary>
    /// <param name="text">O texto (pergunta do usuário) a ser vetorizado.</param>
    /// <returns>Um array de floats representando o vetor semântico do texto.</returns>
    /// <exception cref="Exception">Lançada caso a API de embeddings retorne um erro ou status de falha.</exception>    
    Task<float[]> GenerateQuestionEmbeddingAsync(string text);

    /// <summary>
    /// Realiza uma busca por similaridade no banco de dados para encontrar o contexto mais relevante para a pergunta.
    /// </summary>
    /// <remarks>
    /// Utiliza o operador de distância de cosseno (&lt;=&gt;) do pgvector para ranquear os fragmentos do currículo
    /// que mais se aproximam semanticamente do vetor da pergunta.
    /// </remarks>
    /// <param name="vector">O vetor da pergunta gerado anteriormente.</param>
    /// <returns>Uma string consolidada contendo os 3 fragmentos de texto mais similares encontrados.</returns>
    Task<string> GetSimilarContextAsync(float[] vector);
}
