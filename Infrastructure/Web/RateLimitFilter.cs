using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using IRibeiroForHire.Services.Interfaces;

namespace IRibeiroForHireAPI.Infrastructure.Web;

/// <summary>
/// Filtro de ação responsável por validar e aplicar o limite de requisições (Rate Limit) por visitante.
/// </summary>
/// <param name="rateLimitService">Serviço que gerencia o saldo de perguntas no banco de dados.</param>
/// <param name="visitorTracker">Componente que extrai ou gera a identidade do visitante.</param>
/// <param name="config">Acesso às configurações e mensagens de erro centralizadas.</param>
public class RateLimitFilter(
    IRateLimitService rateLimitService,
    VisitorTracker visitorTracker,
    IConfiguration config
) : IAsyncActionFilter
{
    /// <summary>
    /// Lógica de validação antes da action do controller ser chamada.
    /// </summary>
    /// <param name="context">Contexto da execução da action.</param>
    /// <param name="next">Delegate para prosseguir para a próxima etapa do pipeline.</param>
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate next)
    {
        var visitorId = visitorTracker.GetOrCreateVisitorId();
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var (remaining, ipLocked) = await rateLimitService.GetDailyLimitStatus(visitorId, ip);
        if (ipLocked || remaining <= 0)
        {
            // Limite excedido, retorna status 429
            context.Result = new ObjectResult(new { 
                message = config["Question:LimitReached"], 
                remaining = 0 
            }) { StatusCode = 429 };
            return;
        }

        // Passa o valor de 'remaining' para o Controller via HttpContext
        context.HttpContext.Items["RemainingLimit"] = remaining;

        await next();
    }
}
