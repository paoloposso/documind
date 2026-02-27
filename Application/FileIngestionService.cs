using Documind.Application.Abstractions;
using Documind.Application.Models;
using System.Text.RegularExpressions;
using org.apache.tika.metadata;
using org.apache.tika.parser;
using org.apache.tika.sax;
using org.apache.tika;
using ikvm.io;

namespace Documind.Application;

public partial class FileIngestionService(
    IIngestionService ingestionService,
    IIngestionJobQueue jobQueue,
    IJobStatusService jobStatusService,
    ILogger<FileIngestionService> logger) : IFileIngestionService
{
    private readonly IIngestionService _ingestionService = ingestionService;
    private readonly IIngestionJobQueue _jobQueue = jobQueue;
    private readonly IJobStatusService _jobStatusService = jobStatusService;
    private readonly ILogger<FileIngestionService> _logger = logger;
    private const int MaxChunkSize = 512;
    private const int Overlap = 128;
    private const int BatchSize = 10;

    public async Task<Guid> QueueIngestionAsync(IFormFile file, CancellationToken ct = default)
    {
        var jobId = Guid.NewGuid();
        var tempPath = Path.Combine(Path.GetTempPath(), $"{jobId}_{file.FileName}");

        LogQueuingIngestion(_logger, file.FileName, jobId);

        await using (var stream = new FileStream(tempPath, FileMode.Create))
        {
            await file.CopyToAsync(stream, ct);
        }

        var job = new IngestionJob(jobId, file.FileName, tempPath);
        var status = new JobStatusResponse(jobId, file.FileName, JobStatus.Pending);

        await _jobStatusService.SetStatusAsync(jobId, status, ct);
        await _jobQueue.EnqueueAsync(job);

        return jobId;
    }

    public async Task ProcessFileAsync(Guid jobId, string filePath, string originalFileName, CancellationToken ct = default)
    {
        LogProcessingJob(_logger, jobId, originalFileName);

        try
        {
            await _jobStatusService.SetStatusAsync(jobId, new JobStatusResponse(jobId, originalFileName, JobStatus.Processing), ct);

            var text = await ExtractTextAsync(filePath);

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new InvalidOperationException("No text could be extracted from the file. It might be empty, encrypted, or require OCR.");
            }

            LogExtractedText(_logger, jobId, text.Length);

            var chunkIndex = 1;
            var chunkTuples = ChunkText(text, MaxChunkSize, Overlap)
                .Select(chunk => (Text: chunk, Source: $"{originalFileName}#chunk-{chunkIndex++}"));

            foreach (var batch in chunkTuples.Chunk(BatchSize))
            {
                LogIngestingBatch(_logger, jobId, batch.Length);
                await _ingestionService.IngestBatchAsync(batch, ct);
            }

            await _jobStatusService.SetStatusAsync(jobId, new JobStatusResponse(jobId, originalFileName, JobStatus.Completed), ct);
            LogCompletedJob(_logger, jobId, chunkIndex - 1);
        }
        catch (Exception ex)
        {
            LogFailedJob(_logger, ex, jobId, ex.Message);
            await _jobStatusService.SetStatusAsync(jobId, new JobStatusResponse(jobId, originalFileName, JobStatus.Failed, ex.Message), ct);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    private async Task<string> ExtractTextAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            try
            {
                var tika = new Tika();
                var file = new java.io.File(filePath);
                
                var fileInfo = new FileInfo(filePath);
                _logger.LogInformation("Attempting to extract text from {FilePath} (Size: {Size} bytes)", filePath, fileInfo.Length);

                var contentType = tika.detect(file);
                _logger.LogInformation("Tika detected content type: {ContentType}", contentType);

                var text = tika.parseToString(file);
                
                if (string.IsNullOrWhiteSpace(text))
                {
                    _logger.LogWarning("Tika returned empty text for {ContentType} file", contentType);
                }
                else
                {
                    _logger.LogInformation("Successfully extracted {Length} characters", text.Length);
                }

                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tika parsing failed for file: {FilePath}", filePath);
                throw;
            }
        });
    }


    private static IEnumerable<string> ChunkText(string text, int chunkSize, int overlap)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        var sentences = MyRegex().Split(text).Where(s => !string.IsNullOrWhiteSpace(s));

        var currentChunk = "";
        foreach (var sentence in sentences)
        {
            if (currentChunk.Length + sentence.Length > chunkSize && currentChunk.Length > 0)
            {
                yield return currentChunk.Trim();

                var words = currentChunk.Split(new[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var overlapWords = words.Skip(Math.Max(0, words.Length - overlap));
                currentChunk = string.Join(" ", overlapWords) + " ";
            }

            currentChunk += sentence + " ";
        }

        if (!string.IsNullOrWhiteSpace(currentChunk))
        {
            yield return currentChunk.Trim();
        }
    }

    [GeneratedRegex(@"(?<=[\.!\?])\s+")]
    private static partial Regex MyRegex();
}
