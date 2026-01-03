using IRibeiroForHireAPI.Domain.Entities;

namespace IRibeiroForHireAPI.Domain.Interfaces;

public interface IQuestionService
{
    Task<string> AskQuestionAsync(
        string question, 
        string visitorId, 
        string ip);

    Task<List<QaInteraction>> GetInteractions(string visitorId);
}