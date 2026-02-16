using Documind.Application;
using Documind.Application.Abstractions;
using Documind.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc; // Add this using directive

namespace Documind.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/search")
            .WithTags("Search");

        group.MapGet("/",
                async Task<Results<
                        BadRequest<string>,
                        Ok<IAsyncEnumerable<DocumentRecord>>
                    >>
                    ([FromQuery] string query, [FromServices] ISearchService service) =>
                {
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        return TypedResults.BadRequest("Query cannot be empty.");
                    }

                    return TypedResults.Ok(service.SearchAsync(query));
                })
            .WithName("Search");
    }
}
