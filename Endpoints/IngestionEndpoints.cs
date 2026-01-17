using Microsoft.AspNetCore.Http.HttpResults;
using Documind.Adapters;

namespace Documind.Endpoints;

public static class IngestionEndpoints
{
    public static void MapIngestionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ingest")
                       .WithTags("Knowledge Ingestion");

        // We inject IngestionService directly into the lambda
        group.MapPost("/",
            async Task<Results<BadRequest<string>, Ok<string>>>
                (string text, IngestionService service) =>
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return TypedResults.BadRequest("Text content cannot be empty.");
            }

            await service.IngestAsync(text);
            return TypedResults.Ok("Knowledge ingested successfully.");
        })
        .WithName("IngestText");
    }
}