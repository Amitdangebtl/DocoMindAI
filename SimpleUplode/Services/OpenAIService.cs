using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public OpenAIService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    // Create Embedding 
    public async Task<float[]> CreateEmbedding(string text)
    {
        var apiKey = _config["OpenAI:ApiKey"];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "text-embedding-3-small",
            input = text
        };

        var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(
            "https://api.openai.com/v1/embeddings",
            content
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(x => x.GetSingle())
            .ToArray();
    }

    // 🔹 Final Answer from ChatGPT
    public async Task<string> GetAnswer(string question, List<string> context)
    {
        var apiKey = _config["OpenAI:ApiKey"];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        var systemPrompt = """
         You are an AI assistant answering questions from a document.

         Rules:
         - If the answer is clearly present in the document, answer it.
         - If the topic or keyword appears anywhere in the document,
           explain it in simple human language using the document context.
         - If the question is NOT relevant to the document:
         - Do NOT just say "Answer not found".
         - Politely guide the user about what kind of questions
           can be asked from this document.
         - Suggest 3 example questions based on the document content.
         - Do NOT invent information.
         - Format the answer in plain text with line breaks, not escaped characters.
         - Keep the response clear, helpful, and human-friendly.
""";

        var userPrompt = $"""
Context:
{string.Join("\n---\n", context)}

Question:
{question}
""";

        var body = new
        {
            model = "gpt-4o-mini",  
            temperature = 0.2,       
            messages = new[]
            {
            new { role = "system", content = systemPrompt },
            new { role = "user", content = userPrompt }
        }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync(
            "https://api.openai.com/v1/chat/completions",
            content
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
    }

}
