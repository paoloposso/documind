using Microsoft.Extensions.AI;
using Documind.Domain;
using Microsoft.Extensions.VectorData;
using System.Diagnostics.CodeAnalysis;

namespace Documind.Adapters;

public class SearchService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingService,
    VectorStore vectorStore)
{
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
Justification = "We are providing a manual VectorStoreCollectionDefinition, so reflection is not required.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode",
Justification = "The schema is statically defined via VectorStoreCollectionDefinition.")]
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
            vector = vector[..768];
        }

        var definition = new VectorStoreCollectionDefinition
        {
            Properties = [
                new VectorStoreKeyProperty("Id", typeof(Guid)) { StorageName = "id" },
                new VectorStoreDataProperty("Content", typeof(string)) { StorageName = "content" },
                new VectorStoreDataProperty("Source", typeof(string)) { StorageName = "source" },
                new VectorStoreVectorProperty("Embedding", typeof(ReadOnlyMemory<float>), 768)
                {
                    StorageName = "embedding",
                    DistanceFunction = DistanceFunction.CosineSimilarity,
                    IndexKind = IndexKind.Hnsw
                }
            ]
        };

        var collection = vectorStore.GetCollection<Guid, DocumentRecord>("documents", definition);

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