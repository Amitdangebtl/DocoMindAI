using Grpc.Core;
using Qdrant.Client;
using Qdrant.Client.Grpc;

public class QdrantService
{
    private readonly QdrantClient _client;
    private readonly OpenAIService _openai;

    private const string Collection = "document_vectors";
    private const int VectorSize = 1536;

    public QdrantService(OpenAIService openai, IConfiguration config)
    {
        _openai = openai;

        var qdrantUrl = config["Qdrant:Url"];
        var qdrantApiKey = config["Qdrant:ApiKey"];

        if (string.IsNullOrEmpty(qdrantUrl))
            throw new Exception("Qdrant URL is missing");

        _client = new QdrantClient(
            new Uri(qdrantUrl),
            apiKey: qdrantApiKey
        );
    }

    // 🔹 Ensure Collection Exists
    public async Task EnsureCollection()
    {
        if (await _client.CollectionExistsAsync(Collection))
            return;

        try
        {
            await _client.CreateCollectionAsync(
                Collection,
                new VectorParams
                {
                    Size = VectorSize,
                    Distance = Distance.Cosine
                }
            );
        }
        catch (RpcException ex)
        {
            if (ex.StatusCode == StatusCode.AlreadyExists ||
                ex.StatusCode == StatusCode.InvalidArgument)
            {
                return;
            }

            throw;
        }
    }

    // 🔹 Insert Document Vector
    public async Task Insert(string text, string documentId)
    {
        await EnsureCollection();

        var vector = await _openai.CreateEmbedding(text);

        await _client.UpsertAsync(
            Collection,
            new[]
            {
                new PointStruct
                {
                    Id = new PointId(Guid.NewGuid()),
                    Vectors = vector,
                    Payload =
                    {
                        ["text"] = text,
                        ["documentId"] = documentId
                    }
                }
            }
        );
    }

    // 🔹 Ask Question using RAG
    public async Task<string> AskAI(string question, string documentId)
    {
        await EnsureCollection();

        var vector = await _openai.CreateEmbedding(question);

        var result = await _client.SearchAsync(
            Collection,
            vector,
            filter: new Filter
            {
                Must =
                {
                    new Condition
                    {
                        Field = new FieldCondition
                        {
                            Key = "documentId",
                            Match = new Match
                            {
                                Keyword = documentId
                            }
                        }
                    }
                }
            },
            limit: 5
        );

        var context = result
            .Where(x => x.Payload.ContainsKey("text"))
            .Select(x => x.Payload["text"].StringValue)
            .ToList();

        return await _openai.GetAnswer(question, context);
    }
}