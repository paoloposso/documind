using Microsoft.Extensions.AI;
using Documind.Domain;
using Documind.Application.Abstractions;

namespace Documind.Application;

public class SearchService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingService,
    IDocumentRepository documentRepository) : ISearchService
{
    public async IAsyncEnumerable<DocumentRecord> SearchAsync(string userQuery)
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

        await foreach (var result in documentRepository.SearchAsync(vector, 3))
        {
            yield return result;
        }
    }
}