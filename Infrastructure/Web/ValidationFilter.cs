using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IRibeiroForHireAPI.Infrastructure.Web;

/// <summary>
/// Filtro para interceptar requisições e valida o estado do modelo (ModelState).
/// Garante que o Controller só processe dados que respeitem as DataAnnotations dos DTOs.
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context) {}

    /// <summary>
    /// Verifica se o modelo enviado na requisição é válido antes da execução da action.
    /// </summary>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        // retorna 400 com os detalhes do erro
        if (!context.ModelState.IsValid)
            context.Result = new BadRequestObjectResult(context.ModelState);
    }
}
