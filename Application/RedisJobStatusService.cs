using System.Text.Json;
using Documind.Application.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Documind.Application.Abstractions;

public interface IJobStatusService
{
    Task SetStatusAsync(Guid jobId, JobStatusResponse status, CancellationToken ct = default);
    Task<JobStatusResponse?> GetStatusAsync(Guid jobId, CancellationToken ct = default);
}

public class RedisJobStatusService(IDistributedCache cache) : IJobStatusService
{
    private static readonly DistributedCacheEntryOptions Options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
    };

    public async Task SetStatusAsync(Guid jobId, JobStatusResponse status, CancellationToken ct = default)
    {
        var key = $"job:{jobId}";
        var json = JsonSerializer.Serialize(status);
        await cache.SetStringAsync(key, json, Options, ct);
    }

    public async Task<JobStatusResponse?> GetStatusAsync(Guid jobId, CancellationToken ct = default)
    {
        var key = $"job:{jobId}";
        var json = await cache.GetStringAsync(key, ct);
        return json is null ? null : JsonSerializer.Deserialize<JobStatusResponse>(json);
    }
}