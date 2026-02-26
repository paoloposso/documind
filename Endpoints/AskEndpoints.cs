using Documind.Application.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Documind.Endpoints;

public static class AskEndpoints
{
    public static void MapAskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ask")
                       .WithTags("Ask");

        group.MapGet("/",
            async Task<Results<BadRequest<string>, Ok<IAsyncEnumerable<string>>>>
                (string question, IAskService service, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return TypedResults.BadRequest("Question cannot be empty.");
            }

            return TypedResults.Ok(service.AskStreamingAsync(question, ct));
        })
        .WithName("Ask");
    }
}
