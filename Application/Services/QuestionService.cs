using Microsoft.SemanticKernel;
using IRibeiroForHireAPI.Domain.Interfaces;
using IRibeiroForHireAPI.Domain.Entities;

namespace IRibeiroForHireAPI.Application.Services;

public class QuestionService(
    IQuestionRepository repository,
    Kernel kernel) : IQuestionService
{
    public async Task<List<QaInteraction>> GetInteractions(string visitorId)
        => await repository.GetByVisitorIdAsync(visitorId);

    public async Task<string> AskQuestionAsync(
        string question,
        string visitorId,
        string ip)
    {
        var settings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        string promptTemplate = @"
            Você é o assistente de carreira do Itamar Ribeiro.
    
            DIRETRIZES ESTRITAS:
            1. Sempre use a função 'get_resume_context' para buscar fatos sobre o Itamar.
            2. Se a informação NÃO estiver no currículo, responda educadamente: 'Desculpe, não tenho essa informação específica sobre a trajetória do Itamar'.
            3. NÃO responda sobre outros assuntos (política, culinária, etc). Mantenha o foco profissional.
            4. Se o usuário apenas disser 'Oi', responda cordialmente e se coloque à disposição para falar do Itamar.

            Pergunta: {{$question}}";

        var arguments = new KernelArguments(settings)
        {
            ["question"] = question
        };

        try
        {
            var result = await kernel.InvokePromptAsync(promptTemplate, arguments);
            string answer = result.ToString();

            Console.WriteLine($"DEBUG SK: Resposta bruta do Kernel -> '{answer}'");

            var interaction = new QaInteraction
            {
                VisitorId = visitorId,
                UserIp = ip,
                QuestionText = question,
                AnswerText = answer,
                InteractionTime = DateTime.UtcNow
            };

            await repository.SaveAsync(interaction);

            return answer;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO NO KERNEL: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"INNER EXCEPTION: {ex.InnerException.Message}");

            throw;
        }
    }
}