using Microsoft.AspNetCore.Mvc;
using IRibeiroForHireAPI.Infrastructure.Web;
using IRibeiroForHireAPI.Application.DTOs;
using IRibeiroForHireAPI.Domain.Interfaces;

namespace IRibeiroForHireAPI.Controllers;


/// <summary>
/// Gerencia as interações de perguntas e respostas
/// </summary>
/// <param name="questionService">Lógica de processamento de perguntas</param>
/// <param name="visitorTracker">Identifica e rastreia visitantes únicos</param>
[ApiController]
[Route("api/[controller]")]
public class QuestionController(
    IQuestionService questionService,
    VisitorTracker visitorTracker
) : ControllerBase
{
    /// <summary>
    /// Envia uma pergunta para ser processada pela IA
    /// </summary>
    /// <remarks>
    /// Este endpoint é protegido por um filtro de Rate Limit que limita a quantidade de perguntas por IP/Visitante.
    /// A validação do formato da pergunta é feita automaticamente pelo ValidationFilter global.
    /// </remarks>
    /// <param name="request">Objeto contendo a pergunta enviada pelo usuário.</param>
    /// <returns>Uma resposta contendo o texto gerado pela IA e o saldo de perguntas restantes para o dia.</returns>
    /// <response code="200">Sucesso: Retorna a resposta da IA e o limite atualizado.</response>
    /// <response code="400">Bad Request: A pergunta não atende aos requisitos de validação.</response>
    /// <response code="429">Too Many Requests: O limite diário de perguntas foi atingido.</response>
    [HttpPost("ask")]
    [ServiceFilter(typeof(RateLimitFilter))]
    public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
    {
        var visitorId = visitorTracker.GetOrCreateVisitorId();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // Processa a resposta da IA
        var answer = await questionService.AskQuestionAsync(request.Question, visitorId, ip);

        // Recupera o limite que o filtro calculou
        var previousRemaining = (int)(HttpContext.Items["RemainingLimit"] ?? 0);
        var currentRemaining = Math.Max(0, previousRemaining - 1);

        return Ok(new { answer, remaining = currentRemaining });
    }

    /// <summary>
    /// Recupera o histórico completo de interações do visitante atual.
    /// </summary>
    /// <returns>Uma lista de interações contendo perguntas e respostas anteriores.</returns>
    /// <response code="200">Retorna a lista de histórico (pode ser vazia).</response>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
        => Ok(await questionService.GetInteractions(
                visitorTracker.GetOrCreateVisitorId()));

    /// <summary>
    /// Verifica o estado de saúde da API e do banco de dados (Keep-alive).
    /// </summary>
    /// <remarks>
    /// Este endpoint é utilizado por serviços externos de monitoramento para evitar 
    /// que o Koyeb e o Neon Postgres entrem em modo de hibernação.
    /// </remarks>
    /// <returns>O status atual da conexão e do servidor.</returns>
    /// <response code="200">Retorna se a API e o banco estão operacionais.</response>
    [HttpGet("health")]
    [HttpHead("health")]
    public async Task<IActionResult> Health()
    {
        var isDbUp = await questionService.CheckDatabaseConnection();

        return Ok(new
        {
            status = "Healthy",
            database = isDbUp ? "Online" : "Offline",
            timestamp = DateTime.UtcNow
        });
    }
}
