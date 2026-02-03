using Microsoft.Extensions.AI;
using Documind.Domain;
using Microsoft.Extensions.VectorData;
using System.Diagnostics.CodeAnalysis;

namespace Documind.Adapters;

public class IngestionService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingService,
    VectorStore vectorStore)
{
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
    Justification = "We are providing a manual VectorStoreCollectionDefinition, so reflection is not required.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode",
    Justification = "The schema is statically defined via VectorStoreCollectionDefinition.")]
    public async Task IngestAsync(string text, string source)
    {
        var options = new EmbeddingGenerationOptions
        {
            Dimensions = 768,
            // We keep this as 'AdditionalProperties' as a hint to the provider
            AdditionalProperties = new() { { "task_type", "RETRIEVAL_DOCUMENT" } }
        };

        // 1. This exists and returns ReadOnlyMemory<float>
        var vector = await embeddingService.GenerateVectorAsync(text, options);

        // 2. MANUAL SLICE: 
        // If the API ignored the '768' request and sent 3072, we slice it here.
        // This is the only way to guarantee Postgres won't throw a 22000 error.
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
        await collection.EnsureCollectionExistsAsync();

        var record = new DocumentRecord
        {
            Id = Guid.NewGuid(),
            Content = text,
            Embedding = vector, // This is now guaranteed to be 768
            Source = source
        };

        await collection.UpsertAsync(record);
    }
}