namespace Documind.Application.Abstractions;

public interface IIngestionService
{
    Task IngestAsync(string text, string source, CancellationToken ct = default);
    Task IngestBatchAsync(IEnumerable<(string Text, string Source)> chunks, CancellationToken ct = default);
}
