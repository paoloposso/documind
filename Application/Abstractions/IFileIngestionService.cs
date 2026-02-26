namespace Documind.Application.Abstractions;

public interface IFileIngestionService
{
    Task IngestFileAsync(IFormFile file, CancellationToken ct = default);
}
