using IRibeiroForHireAPI.Domain.Entities;

namespace IRibeiroForHireAPI.Domain.Interfaces;


/// <summary>
/// Repositório responsável pela persistência e consulta das interações de perguntas e respostas.
/// Gerencia o acesso à tabela de interações para fins de histórico e auditoria de limites (Rate Limit).
/// </summary>
/// <param name="context">O contexto de banco de dados do Entity Framework Core.</param>
public interface IQuestionRepository
{
    /// <summary>
    /// Conta o número de interações vinculadas a um ID de visitante único dentro de um determinado dia.
    /// </summary>
    /// <param name="visitorId">O hash identificador do visitante (proveniente do cookie).</param>
    /// <param name="date">A data de referência para a contagem.</param>
    /// <returns>O total de perguntas vinculadas ao visitante na data informada.</returns>    
    Task<int> CountDailyByVisitorIdAsync(string visitorId, DateTime date);

    /// <summary>
    /// Conta o número de interações realizadas por um endereço IP específico dentro de um determinado dia.
    /// </summary>
    /// <param name="ip">O endereço IP do visitante.</param>
    /// <param name="date">A data de referência para a contagem.</param>
    /// <returns>O total de perguntas realizadas pelo IP na data informada.</returns>
    Task<int> CountDailyByIpAsync(string ip, DateTime date);

    /// <summary>
    /// Busca o horário exato da última interação registrada para um determinado endereço IP.
    /// Útil para validações de intervalo mínimo entre requisições.
    /// </summary>
    /// <param name="ip">O endereço IP do visitante.</param>
    /// <returns>O <see cref="DateTime"/> da última interação ou nulo caso não exista registro.</returns>
    Task<DateTime?> GetLastInteractionTimeByIpAsync(string ip);

    /// <summary>
    /// Recupera o histórico completo de perguntas e respostas de um visitante, ordenado pela mais recente.
    /// </summary>
    /// <param name="visitorId">O identificador único do visitante.</param>
    /// <returns>Uma lista de objetos <see cref="QaInteraction"/>.</returns>
    Task<List<QaInteraction>> GetByVisitorIdAsync(string visitorId);

    /// <summary>
    /// Persiste uma nova interação de pergunta e resposta no banco de dados.
    /// </summary>
    /// <param name="interaction">O objeto de interação populado.</param>    
    Task SaveAsync(QaInteraction interaction);

    /// <summary>
    /// Verifica se consegue abrir a conexão ao database.
    /// </summary>
    Task<bool> PingAsync();
}
