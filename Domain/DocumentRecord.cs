using Microsoft.Extensions.VectorData;

namespace Documind.Domain;

public class DocumentRecord
{
    [VectorStoreKey]
    public Guid Id { get; set; }

    [VectorStoreData(IsIndexed = true)]
    public string Content { get; set; } = string.Empty;

    [VectorStoreData]
    public string Source { get; set; } = "Manual Entry";

    // Gemini embedding-004 output is 768 dimensions
    [VectorStoreVector(Dimensions: 768, DistanceFunction = DistanceFunction.CosineSimilarity)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}