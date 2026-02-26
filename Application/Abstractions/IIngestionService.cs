namespace Documind.Application.Abstractions;

public interface IIngestionService
{
    Task IngestAsync(string text, string source, CancellationToken ct = default);
}
