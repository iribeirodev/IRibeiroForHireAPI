namespace IRibeiroForHire.Services.Interfaces;

public interface IRateLimitService
{
    /// <summary>
    /// Calcula os limites de uso diário de um visitante, usando tanto o Visitor ID quanto o IP
    /// </summary>
    /// <param name="visitorId">ID único do visitante</param>
    /// <param name="ip">Endereço IP do usuário</param>
    /// <returns>Tuple contendo o número de perguntas restantes e um flag indicando se o IP está bloqueado</returns>
    Task<(int Remaining, bool IpLocked)> GetDailyLimitStatus(string visitorId, string ip);
}