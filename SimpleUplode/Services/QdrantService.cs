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

        var qdrantUrl = config["QDRANT_URL"];
        var qdrantApiKey = config["QDRANT_API_KEY"];

        _client = new QdrantClient(
            new Uri(qdrantUrl),
            apiKey: qdrantApiKey
        );
    }

    //public QdrantService(OpenAIService openai)
    //{

    //    _client = new QdrantClient("qdrant", 6334);
    //    //_client = new QdrantClient("localhost", 6334);
    //    _openai = openai;
    //}

    // 🔹 Ensure collection exists (SAFE & IDEMPOTENT)
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
            // If already created by another request/thread
            if (ex.StatusCode == StatusCode.AlreadyExists ||
                ex.StatusCode == StatusCode.InvalidArgument)
            {
                return;
            }

            throw;
        }
    }

    // 🔹 Insert document into Qdrant
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

    // 🔹 Ask AI using RAG
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
