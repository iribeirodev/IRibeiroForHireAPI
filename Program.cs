using Microsoft.EntityFrameworkCore;
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

        // Settings
        var geminiSettings = new GeminiOptions
        {
            ApiKey = Env.GetString("GEMINI_API_KEY"),
            Model = Env.GetString("GEMINI_MODEL"),
            SystemPrompt = Env.GetString("AI_SYSTEM_PROMPT"),
            BaseUrl = Env.GetString("GEMINI_BASE_URL"),
            DbConnectionString = Env.GetString("DB_CONNECTION_STRING"),
            Temperature = Env.GetDouble("AI_TEMPERATURE", 0.7),
            MaxTokens = Env.GetInt("AI_MAX_TOKENS", 4000)
        };
        builder.Services.AddSingleton(geminiSettings);

        // Databse
        var connectionString = Env.GetString("DB_CONNECTION_STRING");
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

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:4200", "http://localhost:3000")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
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
        app.UseCors("FrontendPolicy");
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}