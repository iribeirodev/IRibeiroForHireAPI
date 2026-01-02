using IRibeiroForHireAPI.Domain.Interfaces; // Ajustado para o novo namespace
using DotNetEnv;
using IRibeiroForHire.Services.Interfaces;

namespace IRibeiroForHireAPI.Application.Services;

public class RateLimitService : IRateLimitService
{
    private readonly IQuestionService _questionService;
    private readonly int _questionLimit;

    public RateLimitService(IQuestionService questionService)
    {
        _questionService = questionService;
        _questionLimit = Env.GetInt("QUESTION_LIMIT", 5);
    }

    public async Task<(int Remaining, bool IpLocked)> GetDailyLimitStatus(string visitorId, string ip)
    {
        // Certifique-se que esses nomes existem EXATAMENTE assim na IQuestionService
        var usedByVisitor = await _questionService.CountDailyInteractionsByVisitorId(visitorId);
        var usedByIp = await _questionService.CountDailyInteractionsByIp(ip);
        
        // CORREÇÃO AQUI: Nome deve ser igual ao da interface
        var lastByIp = await _questionService.GetLastInteractionByIp(ip);

        int used = Math.Max(usedByVisitor, usedByIp);

        bool ipLocked = usedByIp >= _questionLimit &&
                        lastByIp.HasValue &&
                        lastByIp.Value > DateTime.UtcNow.AddHours(-24);

        int remaining = ipLocked ? 0 : Math.Max(0, _questionLimit - used);

        return (remaining, ipLocked);
    }
}