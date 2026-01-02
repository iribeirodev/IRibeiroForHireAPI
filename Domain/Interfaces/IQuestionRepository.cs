using IRibeiroForHireAPI.Domain.Entities;

namespace IRibeiroForHireAPI.Domain.Interfaces;

/// <summary>
/// Tratamento de leitura e persistência das perguntas
/// </summary>
public interface IQuestionRepository
{
    Task<int> CountDailyByVisitorIdAsync(string visitorId, DateTime date);
    Task<int> CountDailyByIpAsync(string ip, DateTime date);
    Task<DateTime?> GetLastInteractionTimeByIpAsync(string ip);
    Task<List<QaInteraction>> GetByVisitorIdAsync(string visitorId);
    Task SaveAsync(QaInteraction interaction);
}
