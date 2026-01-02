using IRibeiroForHireAPI.Domain.Interfaces;
using IRibeiroForHireAPI.Domain.Entities;
using IRibeiroForHire.Services.Interfaces;

namespace IRibeiroForHireAPI.Application.Services;

// Usando Primary Constructor corretamente (sem redeclarar campos privados)
public class QuestionService(
    IQuestionRepository repository,
    IGeminiTextService textGenerator) : IQuestionService
{
    public async Task<int> CountDailyInteractionsByVisitorId(string visitorId)
    {
        return await repository.CountDailyByVisitorIdAsync(visitorId, DateTime.UtcNow);
    }

    public async Task<int> CountDailyInteractionsByIp(string ip)
    {
        return await repository.CountDailyByIpAsync(ip, DateTime.UtcNow);
    }

    public async Task<DateTime?> GetLastInteractionByIp(string ip)
    {
        return await repository.GetLastInteractionTimeByIpAsync(ip);
    }

    public async Task<List<QaInteraction>> GetInteractions(string visitorId)
    {
        return await repository.GetByVisitorIdAsync(visitorId);
    }

    public async Task SaveInteraction(QaInteraction interaction)
    {
        await repository.SaveAsync(interaction);
    }

    public async Task<string> AskQuestionAsync(string context, string question, string visitorId, string ip)
    {
        // Usa o parâmetro do construtor diretamente
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