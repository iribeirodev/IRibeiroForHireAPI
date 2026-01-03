using Microsoft.EntityFrameworkCore;
using IRibeiroForHire.Infrastructure.Data;
using IRibeiroForHireAPI.Domain.Entities;
using IRibeiroForHireAPI.Domain.Interfaces;

namespace IRibeiroForHireAPI.Infrastructure.Repositories;

public class QuestionRepository(AppDbContext context) : IQuestionRepository
{
    public async Task<int> CountDailyByIpAsync(
        string ip, 
        DateTime date)
    {
        var (start, end) = GetDayRange(date);

        return await context.QaInteractions
                            .CountAsync(i => i.UserIp == ip
                                            && i.InteractionTime >= start
                                            && i.InteractionTime < end);
    }

    public async Task<int> CountDailyByVisitorIdAsync(
        string visitorId, 
        DateTime date)
    {
        var (start, end) = GetDayRange(date);

        return await context.QaInteractions
                                .CountAsync(i => i.VisitorId == visitorId
                                                && i.InteractionTime >= start
                                                && i.InteractionTime < end);
    }

    public async Task<List<QaInteraction>> GetByVisitorIdAsync(string visitorId)
        => await context.QaInteractions
                            .Where(q => q.VisitorId == visitorId)
                            .OrderByDescending(q => q.InteractionTime)
                            .ToListAsync();

    public async Task<DateTime?> GetLastInteractionTimeByIpAsync(string ip)
        => await context.QaInteractions
                    .Where(i => i.UserIp == ip)
                    .OrderByDescending(i => i.InteractionTime)
                    .Select(i => (DateTime?)i.InteractionTime)
                    .FirstOrDefaultAsync();

    public async Task SaveAsync(QaInteraction interaction)
    {
        context.QaInteractions.Add(interaction);
        await context.SaveChangesAsync();
    }

    #region Private Methods
    /// <summary>
    /// Calcula o intervalo de tempo cobrindo um dia inteiro (00:00:00 até 23:59:59).
    /// Usado para garantir que a consulta SQL capture todas as interações do dia,
    /// independentemente do horário (HH:mm:ss) gravado no banco.
    /// </summary>
    private (DateTime inicio, DateTime fim) GetDayRange(DateTime date)
    {
        var start = date.Date;
        return (start, start.AddDays(1));
    }
    #endregion
}
