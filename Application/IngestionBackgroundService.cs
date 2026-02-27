using Documind.Application.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Documind.Application;

public partial class IngestionBackgroundService(
    IIngestionJobQueue jobQueue,
    IServiceScopeFactory scopeFactory,
    ILogger<IngestionBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogStarting(logger);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await jobQueue.DequeueAsync(stoppingToken);
                
                LogProcessingJob(logger, job.Id, job.FileName);

                using var scope = scopeFactory.CreateScope();
                var ingestionService = scope.ServiceProvider.GetRequiredService<IFileIngestionService>();

                await ingestionService.ProcessFileAsync(job.Id, job.TempFilePath, job.FileName, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                LogErrorProcessing(logger, ex);
            }
        }

        LogStopping(logger);
    }
}