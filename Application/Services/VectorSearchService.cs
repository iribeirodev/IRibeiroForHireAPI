using Npgsql;
using IRibeiroForHire.Services.Interfaces;
using IRibeiroForHireAPI.Infrastructure.Configurations;

namespace IRibeiroForHireAPI.Application.Services;

public class VectorSearchService(HttpClient httpClient, GeminiOptions options) : IVectorSearchService
{
    public async Task<float[]> GenerateQuestionEmbeddingAsync(string text)
    {
        string baseUrl = options.BaseUrl.EndsWith("/") ? options.BaseUrl : options.BaseUrl + "/";
        string url = $"{baseUrl}models/text-embedding-004:embedContent?key={options.ApiKey}";

        var request = new { content = new { parts = new[] { new { text } } } };

        var response = await httpClient.PostAsJsonAsync(url, request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro ao gerar embedding: {response.StatusCode} - {errorBody}");
        }

        var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();

        // Extrai o vetor (768 dimensões)
        var values = result.GetProperty("embedding").GetProperty("values");

        float[] vector = new float[768];
        int i = 0;
        foreach (var val in values.EnumerateArray())
            vector[i++] = val.GetSingle();

        return vector;
    }

    public async Task<string> GetSimilarContextAsync(float[] vector)
    {
        var chunks = new List<string>();
        
        await using var conn = new NpgsqlConnection(options.DbConnectionString);
        await conn.OpenAsync();

        // Faz busca por similaridade de cosseno no pgvector
        string sql = "SELECT chunk_text FROM resume_chunks ORDER BY embedding <=> @v::vector LIMIT 3";
        
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("v", vector);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) 
        {
            chunks.Add(reader.GetString(0));
        }

        return string.Join("\n", chunks);
    }
}