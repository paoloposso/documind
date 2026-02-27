using System.Threading.Channels;
using Documind.Application.Models;

namespace Documind.Application.Abstractions;

public interface IIngestionJobQueue
{
    ValueTask EnqueueAsync(IngestionJob job);
    ValueTask<IngestionJob> DequeueAsync(CancellationToken ct);
}

public class IngestionJobQueue : IIngestionJobQueue
{
    private readonly Channel<IngestionJob> _channel = Channel.CreateUnbounded<IngestionJob>();

    public async ValueTask EnqueueAsync(IngestionJob job)
    {
        await _channel.Writer.WriteAsync(job);
    }

    public async ValueTask<IngestionJob> DequeueAsync(CancellationToken ct)
    {
        return await _channel.Reader.ReadAsync(ct);
    }
}