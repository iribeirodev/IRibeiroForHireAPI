using Npgsql;
using DotNetEnv;
using IRibeiroForHire.Services.Interfaces;

namespace IRibeiroForHireAPI.Application.Services;

public class VectorSearchService : IVectorSearchService
{
    private readonly string _connectionString;
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public VectorSearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _connectionString = Env.GetString("DB_CONNECTION_STRING");
        _apiKey = Env.GetString("GEMINI_API_KEY");
    }

    public async Task<float[]> GenerateQuestionEmbeddingAsync(string text)
    {
        // A URL de embedding também usa a chave do .env
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/text-embedding-004:embedContent?key={_apiKey}";
        var request = new { content = new { parts = new[] { new { text } } } };

        var response = await _httpClient.PostAsJsonAsync(url, request);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao gerar embedding: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();

        // Converte o retorno para array de float (768 dimensões para o text-embedding-004)
        var values = result.GetProperty("embedding").GetProperty("values");
        float[] vector = new float[768];
        int i = 0;

        foreach (var val in values.EnumerateArray())
        {
            vector[i++] = val.GetSingle();
        }

        return vector;
    }

    public async Task<string> GetSimilarContextAsync(float[] vector)
    {
        var chunks = new List<string>();
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Busca os 3 trechos mais similares usando a distância de cosseno (<=>) do pgvector
        string sql = "SELECT chunk_text FROM resume_chunks ORDER BY embedding <=> @v::vector LIMIT 3";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("v", vector);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) chunks.Add(reader.GetString(0));

        return string.Join("\n", chunks);
    }
}