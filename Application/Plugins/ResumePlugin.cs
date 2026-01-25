using IRibeiroForHire.Services.Interfaces;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace IRibeiroForHireAPI.Application.Plugins;

public class ResumePlugin(IVectorSearchService vectorSearch)
{
    [KernelFunction("get_resume_context")]
    [Description("Busca informações relevantes no currículo do Itamar Ribeiro com base em uma pergunta.")]
    public async Task<string> GetResumeContextAsync(
        [Description("A pergunta ou termo de busca para localizar no currículo")] string question)
    {
        // Gera o embedding da pergunta e busca no pgvector as partes com maior similaridade
        var vector = await vectorSearch.GenerateQuestionEmbeddingAsync(question);

        string context = await vectorSearch.GetSimilarContextAsync(vector);

        return string.IsNullOrEmpty(context)
            ? "Informações gerais sobre o Itamar Ribeiro."
            : context;
    }
}