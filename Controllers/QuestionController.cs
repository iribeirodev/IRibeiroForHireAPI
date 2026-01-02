using Microsoft.AspNetCore.Mvc;
using IRibeiroForHireAPI.Infrastructure.Web;
using IRibeiroForHireAPI.Application.DTOs;
using IRibeiroForHireAPI.Domain.Interfaces;

namespace IRibeiroForHireAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController(
    IQuestionService questionService,
    VisitorTracker visitorTracker) : ControllerBase
{
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] QuestionRequest request)
    {
        if (string.IsNullOrEmpty(request.Question))
            return BadRequest("A pergunta não pode estar vazia.");

        // Processa a pergunta e salva a interação
        var answer = await questionService.AskQuestionAsync(
            context: "Você é um assistente que ajuda recrutadores a conhecerem o trabalho do Itamar Ribeiro.",
            question: request.Question, 
            visitorId: visitorTracker.GetOrCreateVisitorId(), 
            ip: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");

        return Ok(new { answer });
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
