using Documind.Application.Abstractions;
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
                async Task<Results<BadRequest<string>, Ok<string>>>
                    (IFormFile file, [FromServices] IFileIngestionService service, CancellationToken ct) =>
                {
                    if (file == null || file.Length == 0)
                    {
                        return TypedResults.BadRequest("File cannot be empty.");
                    }

                    await service.IngestFileAsync(file, ct);
                    return TypedResults.Ok("File ingested successfully.");
                })
            .WithName("IngestFile")
            .DisableAntiforgery();
    }
}
