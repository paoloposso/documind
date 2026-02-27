namespace Documind.Application.Models;

public record IngestionJob(Guid Id, string FileName, string TempFilePath);

public enum JobStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

public record JobStatusResponse(Guid Id, string FileName, JobStatus Status, string? ErrorMessage = null);