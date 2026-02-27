using Documind.Application.Abstractions;
using Documind.Application.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Documind.Endpoints;

public static class FileIngestionEndpoints
{
    public static void MapFileIngestionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ingest-file")
            .WithTags("Knowledge Ingestion");

        group.MapPost("/",
                async Task<Results<BadRequest<string>, Accepted<Guid>>>
                    (IFormFile file, [FromServices] IFileIngestionService service, CancellationToken ct) =>
                {
                    if (file == null || file.Length == 0)
                    {
                        return TypedResults.BadRequest("File cannot be empty.");
                    }

                    var jobId = await service.QueueIngestionAsync(file, ct);
                    return TypedResults.Accepted($"/api/ingest-file/status/{jobId}", jobId);
                })
            .WithName("IngestFile")
            .DisableAntiforgery();

        group.MapGet("/status/{jobId:guid}",
                async Task<Results<NotFound, Ok<JobStatusResponse>>>
                    (Guid jobId, [FromServices] IJobStatusService service, CancellationToken ct) =>
                {
                    var status = await service.GetStatusAsync(jobId, ct);
                    return status is null ? TypedResults.NotFound() : TypedResults.Ok(status);
                })
            .WithName("GetIngestionStatus");
    }
}
