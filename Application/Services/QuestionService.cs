using Microsoft.SemanticKernel;
using IRibeiroForHireAPI.Domain.Interfaces;
using IRibeiroForHireAPI.Domain.Entities;

namespace IRibeiroForHireAPI.Application.Services;

public class QuestionService(
    IQuestionRepository repository,
    Kernel kernel,
    IConfiguration configuration) : IQuestionService
{
    public async Task<List<QaInteraction>> GetInteractions(string visitorId)
        => await repository.GetByVisitorIdAsync(visitorId);

    public async Task<string> AskQuestionAsync(
        string question,
        string visitorId,
        string ip)
    {
        string systemPrompt = configuration["AI_SYSTEM_PROMPT"]
            ?? "Você é o Assistente Virtual do Itamar Ribeiro.";

        var settings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        string promptTemplate = $@"{systemPrompt}

            Pergunta do Recrutador: {{$question}}";

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