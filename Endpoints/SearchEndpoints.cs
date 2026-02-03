using Documind.Adapters;
using Documind.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Documind.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/search")
            .WithTags("Search");

        group.MapGet("/",
                async Task<Results<BadRequest<string>, Ok<List<DocumentRecord>>>>
                    (string query, SearchService service) =>
                {
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        return TypedResults.BadRequest("Query cannot be empty.");
                    }

                    var results = await service.SearchAsync(query);
                    return TypedResults.Ok(results);
                })
            .WithName("Search");
    }
}
