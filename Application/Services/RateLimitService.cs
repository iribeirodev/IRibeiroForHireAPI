using IRibeiroForHireAPI.Domain.Interfaces;
using DotNetEnv;
using IRibeiroForHire.Services.Interfaces;

namespace IRibeiroForHireAPI.Application.Services;

public class RateLimitService(IQuestionRepository repository) : IRateLimitService
{
    private readonly int _questionLimit = Env.GetInt("QUESTION_LIMIT", 5);

    public async Task<(int Remaining, bool IpLocked)> GetDailyLimitStatus(string visitorId, string ip)
    {
        var usedByVisitor = await repository.CountDailyByVisitorIdAsync(visitorId, DateTime.UtcNow);
        var usedByIp = await repository.CountDailyByIpAsync(ip, DateTime.UtcNow);
        var lastByIp = await repository.GetLastInteractionTimeByIpAsync(ip);

        int used = Math.Max(usedByVisitor, usedByIp);

        bool ipLocked = usedByIp >= _questionLimit &&
                        lastByIp.HasValue &&
                        lastByIp.Value > DateTime.UtcNow.AddHours(-24);

        int remaining = ipLocked ? 0 : Math.Max(0, _questionLimit - used);

        return (remaining, ipLocked);
    }
}