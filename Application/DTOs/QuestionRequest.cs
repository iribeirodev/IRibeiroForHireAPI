using System.ComponentModel.DataAnnotations;

namespace IRibeiroForHireAPI.Application.DTOs;

/// <summary>
/// Representa a requisição enviada pelo usuário ao realizar uma pergunta para a IA.
/// </summary>
public class QuestionRequest
{
/// <summary>
    /// O conteúdo da pergunta ou mensagem enviada pelo visitante.
    /// </summary>
    /// <example>Quem é o Itamar Ribeiro?</example>
    [Required(ErrorMessage = "A pergunta não pode estar vazia.")]    
    public string Question { get; set; } = string.Empty;
}
