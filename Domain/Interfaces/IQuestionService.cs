using IRibeiroForHireAPI.Domain.Entities;

namespace IRibeiroForHireAPI.Domain.Interfaces;

public interface IQuestionService
{
    Task<string> AskQuestionAsync(
        string context, 
        string question, 
        string visitorId, 
        string ip);

    Task<int> CountDailyInteractionsByVisitorId(string visitorId);

    Task<int> CountDailyInteractionsByIp(string ip);

    Task<DateTime?> GetLastInteractionByIp(string ip);

    Task<List<QaInteraction>> GetInteractions(string visitorId);
}