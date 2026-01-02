using DotNetEnv;
using IRibeiroForHire.Infrastructure.Data;
using IRibeiroForHire.Services.Interfaces;
using IRibeiroForHireAPI.Application.Services;
using IRibeiroForHireAPI.Domain.Interfaces;
using IRibeiroForHireAPI.Infrastructure.Configurations;
using IRibeiroForHireAPI.Infrastructure.ExternalServices;
using IRibeiroForHireAPI.Infrastructure.Repositories;
using IRibeiroForHireAPI.Infrastructure.Web;
using Microsoft.EntityFrameworkCore;

namespace IRibeiroForHireAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        DotNetEnv.Env.Load();

        // Configurações da IA
        var geminiSettings = new GeminiOptions
        {
            ApiKey = Env.GetString("GEMINI_API_KEY"),
            Model = Env.GetString("GEMINI_MODEL"),
            SystemPrompt = Env.GetString("AI_SYSTEM_PROMPT"),
            Temperature = Env.GetDouble("AI_TEMPERATURE", 0.7),
            MaxTokens = Env.GetInt("AI_MAX_TOKENS", 1000)
        };
        builder.Services.AddSingleton(geminiSettings);

        // Databsae
        var connectionString = Env.GetString("DATABASE_URL");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)); // Ou UseSqlServer se for o caso

        // Registro dos Componentes (Injeção de Dependência)
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<VisitorTracker>();
        
        // Camada de Infrastructure (Adaptadores)
        builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
        builder.Services.AddHttpClient<IGeminiTextService, GeminiTextService>();

        // Camada de Application (Casos de Uso)
        builder.Services.AddScoped<IQuestionService, QuestionService>();

        // 4. Configuração de CORS para Angular/React
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:4200", "http://localhost:3000") // Angular e React
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials(); // Para uso do VisitorTracker (cookies)
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        
        // Aplica a política de CORS antes da autorização
        app.UseCors("FrontendPolicy");

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}