namespace Documind.Application.Abstractions;

public interface IFileIngestionService
{
    Task<Guid> QueueIngestionAsync(IFormFile file, CancellationToken ct = default);
    Task ProcessFileAsync(Guid jobId, string filePath, string originalFileName, CancellationToken ct = default);
}
