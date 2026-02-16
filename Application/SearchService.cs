using Microsoft.Extensions.AI;
using Documind.Domain;
using Documind.Application.Abstractions;
using System.Runtime.CompilerServices;

namespace Documind.Application;

public class SearchService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingService,
    IDocumentRepository documentRepository) : ISearchService
{
    public async IAsyncEnumerable<DocumentRecord> SearchAsync(
        string userQuery,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var options = new EmbeddingGenerationOptions
        {
            Dimensions = 768,
            AdditionalProperties = new() { { "task_type", "RETRIEVAL_QUERY" } }
        };

        var vector = await embeddingService.GenerateVectorAsync(userQuery, options, ct);

        if (vector.Length > 768)
        {
            vector = vector[..768];
        }

        await foreach (var result in documentRepository.SearchAsync(vector, 3).WithCancellation(ct))
        {
            yield return result;
        }
    }
}