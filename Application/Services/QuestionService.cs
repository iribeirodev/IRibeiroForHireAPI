using IRibeiroForHireAPI.Domain.Interfaces;
using IRibeiroForHireAPI.Domain.Entities;
using IRibeiroForHire.Services.Interfaces;

namespace IRibeiroForHireAPI.Application.Services;

public class QuestionService(
    IQuestionRepository repository,
    IGeminiTextService textGenerator,
    IVectorSearchService vectorSearch) : IQuestionService
{
    public async Task<List<QaInteraction>> GetInteractions(string visitorId)
        => await repository.GetByVisitorIdAsync(visitorId);

    public async Task<string> AskQuestionAsync(
        string question, 
        string visitorId, 
        string ip)
    {
        // vetor da pergunta
        var vector = await vectorSearch.GenerateQuestionEmbeddingAsync(question);
        // busca no pgvector
        string context = await vectorSearch.GetSimilarContextAsync(vector);

        if (string.IsNullOrEmpty(context))
            context = "Informações gerais sobre o Itamar Ribeiro.";

        // interação com I.A.
        string answer = await textGenerator.GenerateAnswerAsync(context, question);

        var interaction = new QaInteraction
        {
            VisitorId = visitorId,
            UserIp = ip,
            QuestionText = question,
            AnswerText = answer,
            InteractionTime = DateTime.UtcNow
        };

        await repository.SaveAsync(interaction);

        return answer;
    }
}