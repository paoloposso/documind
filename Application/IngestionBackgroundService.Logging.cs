using Microsoft.Extensions.Logging;

namespace Documind.Application;

public partial class IngestionBackgroundService
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Ingestion Background Service is starting.")]
    static partial void LogStarting(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Ingestion Background Service is stopping.")]
    static partial void LogStopping(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Processing job {JobId} for file: {FileName}")]
    static partial void LogProcessingJob(ILogger logger, Guid jobId, string fileName);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error processing ingestion job.")]
    static partial void LogErrorProcessing(ILogger logger, Exception ex);
}