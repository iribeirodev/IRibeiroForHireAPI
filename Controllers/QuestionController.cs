using Microsoft.AspNetCore.Mvc;
using IRibeiroForHireAPI.Infrastructure.Web;
using IRibeiroForHireAPI.Application.DTOs;
using IRibeiroForHireAPI.Domain.Interfaces;
using IRibeiroForHire.Services.Interfaces;

namespace IRibeiroForHireAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController(
    IQuestionService questionService,
    IRateLimitService rateLimitService,
    VisitorTracker visitorTracker) : ControllerBase
{
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
    {
        if (string.IsNullOrEmpty(request.Question))
            return BadRequest("A pergunta não pode estar vazia.");

        // Identifica o visitante
        var visitorId = visitorTracker.GetOrCreateVisitorId();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var (remaining, ipLocked) = await rateLimitService.GetDailyLimitStatus(visitorId, ip);

        if (ipLocked || remaining <= 0)
        {
            return StatusCode(429, new { 
                message = "Limite diário de perguntas atingido. Tente novamente em 24 horas.",
                remaining = 0
            });
        }

        // Processa a pergunta e salva a interação
        var answer = await questionService.AskQuestionAsync(
            question: request.Question, 
            visitorId: visitorId, 
            ip: ip);

        return Ok(new { answer, remaining = remaining - 1 });
    }

    /// <summary>
    /// Recupera o Id do visitante para buscar o histórico dele
    /// </summary>
    /// <returns></returns>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var history = await questionService.GetInteractions(
            visitorId: visitorTracker.GetOrCreateVisitorId()
        );
        
        return Ok(history);
    }    
}
