using Microsoft.Extensions.VectorData;

namespace Documind.Domain;

public class DocumentRecord
{
    [VectorStoreKey(StorageName = "id")]
    public Guid Id { get; set; }

    [VectorStoreData(StorageName = "content", IsIndexed = true)]
    public string Content { get; set; } = string.Empty;

    [VectorStoreData(StorageName = "source")]
    public string Source { get; set; } = "Manual Entry";

    // We use 768 to stay under the 2000-dimension HNSW limit
    [VectorStoreVector(768, StorageName = "embedding", DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}