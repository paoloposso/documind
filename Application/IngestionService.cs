using Microsoft.Extensions.AI;
using Documind.Domain;
using Documind.Application.Abstractions;

namespace Documind.Application;

public class IngestionService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingService,
    IDocumentRepository documentRepository) : IIngestionService
{
    public async Task IngestAsync(string text, string source, CancellationToken ct = default)
    {
        var options = new EmbeddingGenerationOptions
        {
            Dimensions = 768,
            // We keep this as 'AdditionalProperties' as a hint to the provider
            AdditionalProperties = new() { { "task_type", "RETRIEVAL_DOCUMENT" } }
        };

        // 1. This exists and returns ReadOnlyMemory<float>
        var vector = await embeddingService.GenerateVectorAsync(text, options, ct);

        // 2. MANUAL SLICE: 
        // If the API ignored the '768' request and sent 3072, we slice it here.
        // This is the only way to guarantee Postgres won't throw a 22000 error.
        if (vector.Length > 768)
        {
            vector = vector[..768];
        }

        var record = new DocumentRecord
        {
            Id = Guid.NewGuid(),
            Content = text,
            Embedding = vector, // This is now guaranteed to be 768
            Source = source
        };

        await documentRepository.AddAsync(record, ct);
    }
}