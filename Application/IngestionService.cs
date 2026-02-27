using Microsoft.Extensions.AI;
using Documind.Domain;
using Documind.Application.Abstractions;
using System.Linq;

namespace Documind.Application;

public class IngestionService(
    IEmbeddingGenerator<string, Embedding<float>> embeddingService,
    IDocumentRepository documentRepository) : IIngestionService
{
    public async Task IngestAsync(string text, string source, CancellationToken ct = default)
    {
        await IngestBatchAsync([(text, source)], ct);
    }

    public async Task IngestBatchAsync(IEnumerable<(string Text, string Source)> chunks, CancellationToken ct = default)
    {
        var options = new EmbeddingGenerationOptions
        {
            Dimensions = 768,
            AdditionalProperties = new() { { "task_type", "RETRIEVAL_DOCUMENT" } }
        };

        var chunkList = chunks.ToList();
        if (chunkList.Count == 0) return;

        var texts = chunkList.Select(c => c.Text).ToList();
        var embeddings = await embeddingService.GenerateAsync(texts, options, ct);

        var records = new List<DocumentRecord>();
        for (int i = 0; i < chunkList.Count; i++)
        {
            var vector = embeddings[i].Vector;
            if (vector.Length > 768)
            {
                vector = vector[..768];
            }

            records.Add(new DocumentRecord
            {
                Id = Guid.NewGuid(),
                Content = chunkList[i].Text,
                Embedding = vector,
                Source = chunkList[i].Source
            });
        }

        // We should add a BatchAddAsync to IDocumentRepository for even better performance
        foreach (var record in records)
        {
            await documentRepository.AddAsync(record, ct);
        }
    }
}