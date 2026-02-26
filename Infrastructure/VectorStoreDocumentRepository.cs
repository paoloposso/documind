using Documind.Domain;
using Microsoft.Extensions.VectorData;
using System.Diagnostics.CodeAnalysis;

namespace Documind.Infrastructure;

public class VectorStoreDocumentRepository(VectorStore vectorStore) : IDocumentRepository
{
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "We are providing a manual VectorStoreCollectionDefinition, so reflection is not required.")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode",
        Justification = "The schema is statically defined via VectorStoreCollectionDefinition.")]
    private VectorStoreCollection<Guid, DocumentRecord> GetDocumentCollection()
    {
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
        return vectorStore.GetCollection<Guid, DocumentRecord>("documents", definition);
    }

    public async Task AddAsync(DocumentRecord record, CancellationToken ct = default)
    {
        var collection = GetDocumentCollection();
        await collection.EnsureCollectionExistsAsync(ct);
        await collection.UpsertAsync(record, ct);
    }

    public async IAsyncEnumerable<DocumentRecord> SearchAsync(ReadOnlyMemory<float> embedding, int limit)
    {
        var collection = GetDocumentCollection();
        var searchOptions = new VectorSearchOptions<DocumentRecord>
        {
            IncludeVectors = false,
            VectorProperty = r => r.Embedding
        };

        var searchResult = collection.SearchAsync(
            embedding,
            limit,
            searchOptions);

        await foreach (var result in searchResult)
        {
            yield return result.Record;
        }
    }
}
