using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using DotNetEnv;
using IRibeiroForHire.Infrastructure.Data;
using IRibeiroForHire.Services.Interfaces;
using IRibeiroForHireAPI.Application.Services;
using IRibeiroForHireAPI.Domain.Interfaces;
using IRibeiroForHireAPI.Infrastructure.Configurations;
using IRibeiroForHireAPI.Infrastructure.Repositories;
using IRibeiroForHireAPI.Infrastructure.Web;
using IRibeiroForHireAPI.Infrastructure.ExternalServices;

namespace IRibeiroForHireAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        DotNetEnv.Env.Load();

        var geminiSettings = new GeminiOptions
        {
            ApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? Env.GetString("GEMINI_API_KEY"),
            Model = Environment.GetEnvironmentVariable("GEMINI_MODEL") ?? Env.GetString("GEMINI_MODEL"),
            SystemPrompt = Environment.GetEnvironmentVariable("AI_SYSTEM_PROMPT") ?? Env.GetString("AI_SYSTEM_PROMPT"),
            BaseUrl = Environment.GetEnvironmentVariable("GEMINI_BASE_URL") ?? Env.GetString("GEMINI_BASE_URL"),
            DbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? Env.GetString("DB_CONNECTION_STRING"),
            Temperature = Env.GetDouble("AI_TEMPERATURE", 0.7),
            MaxTokens = Env.GetInt("AI_MAX_TOKENS", 4000)
        };
        builder.Services.AddSingleton(geminiSettings);

        // Database
        var connectionString = geminiSettings.DbConnectionString;
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Infrastructure
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<VisitorTracker>();
        builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

        builder.Services.AddHttpClient<IGeminiTextService, GeminiTextService>();
        builder.Services.AddHttpClient<IVectorSearchService, VectorSearchService>();

        builder.Services.AddScoped<IQuestionService, QuestionService>();
        builder.Services.AddScoped<IRateLimitService, RateLimitService>();

        // CORS - Ajustado para aceitar o frontend no Koyeb
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.WithOrigins(
                    "https://iribeiroforhire.vercel.app",
                    "https://iribeiro.tec.br",
                    "https://www.iribeiro.tec.br"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            // Limpa os proxies e redes conhecidas para aceitar cabeçalhos da Koyeb
            options.KnownProxies.Clear();
            options.KnownIPNetworks.Clear();
        });

        var app = builder.Build();

        app.UseForwardedHeaders();

        // Configuração de Porta para o Koyeb
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        app.Urls.Add($"http://0.0.0.0:{port}");

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("FrontendPolicy");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}