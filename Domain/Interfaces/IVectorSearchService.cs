namespace IRibeiroForHire.Services.Interfaces;

public interface IVectorSearchService
{
    /// <summary>
    /// Gera o vetor da pergunta do usuário
    /// </summary>
    Task<float[]> GenerateQuestionEmbeddingAsync(string text);

    /// <summary>
    /// Busca no Postgres os trechos mais similares
    /// </summary>
    Task<string> GetSimilarContextAsync(float[] vector);
}
