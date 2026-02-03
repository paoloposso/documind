using Microsoft.Extensions.AI;
using Documind.Domain;
using Microsoft.Extensions.VectorData;

namespace Documind.Adapters;

public class SearchService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingService,
    VectorStore vectorStore)
{
    public async Task<List<DocumentRecord>> SearchAsync(string userQuery)
    {
        var options = new EmbeddingGenerationOptions
        {
            Dimensions = 768,
            AdditionalProperties = new() { { "task_type", "RETRIEVAL_QUERY" } }
        };

        var vector = await embeddingService.GenerateVectorAsync(userQuery, options);

        if (vector.Length > 768)
        {
            vector = vector.Slice(0, 768);
        }

        var collection = vectorStore.GetCollection<Guid, DocumentRecord>("documents");

        var searchOptions = new VectorSearchOptions<DocumentRecord>
        {
            IncludeVectors = false,
            VectorProperty = r => r.Embedding
        };

        // 2. The call: Vector, then Top (3), then Options
        var searchResult = collection.SearchAsync<ReadOnlyMemory<float>>(
            vector,
            3,
            searchOptions);

        var results = new List<DocumentRecord>();

        await foreach (var result in searchResult)
        {
            results.Add(result.Record);
        }

        return results;
    }
}