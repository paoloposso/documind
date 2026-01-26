using Microsoft.Extensions.AI; // New namespace for IEmbeddingGenerator
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
    public async Task IngestAsync(string text)
    {
        var definition = new VectorStoreCollectionDefinition
        {
            Properties = [
                new VectorStoreKeyProperty("Id", typeof(Guid))
                {
                    StorageName = "id"
                },
                new VectorStoreDataProperty("Content", typeof(string))
                {
                    StorageName = "content"
                },
                new VectorStoreDataProperty("Source", typeof(string))
                {
                    StorageName = "source"
                },
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

        var vector = await embeddingService.GenerateVectorAsync(text);
        var record = new DocumentRecord
        {
            Id = Guid.NewGuid(),
            Content = text,
            Embedding = vector
        };

        await collection.UpsertAsync(record);
    }
}