using Microsoft.Extensions.AI;
using Documind.Domain;

namespace Documind.Application;

public class SearchService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingService,
    IDocumentRepository documentRepository) : ISearchService
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
            vector = vector[..768];
        }

        var results = new List<DocumentRecord>();

        await foreach (var result in documentRepository.SearchAsync(vector, 3))
        {
            results.Add(result);
        }

        return results;
    }
}