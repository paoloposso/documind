using Documind.Application.Abstractions;

namespace Documind.Application;

public partial class FileIngestionService : IFileIngestionService
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Queuing ingestion for file: {FileName} with JobId: {JobId}")]
    static partial void LogQueuingIngestion(ILogger logger, string fileName, Guid jobId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Processing Job {JobId}: {FileName}")]
    static partial void LogProcessingJob(ILogger logger, Guid jobId, string fileName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Job {JobId}: Extracted {TextLength} characters")]
    static partial void LogExtractedText(ILogger logger, Guid jobId, int textLength);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Job {JobId}: Ingesting batch of {BatchSize} chunks")]
    static partial void LogIngestingBatch(ILogger logger, Guid jobId, int batchSize);

    [LoggerMessage(Level = LogLevel.Information, Message = "Completed Job {JobId}. Total chunks: {TotalChunks}")]
    static partial void LogCompletedJob(ILogger logger, Guid jobId, int totalChunks);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed Job {JobId}: {ErrorMessage}")]
    static partial void LogFailedJob(ILogger logger, Exception ex, Guid jobId, string errorMessage);
}
