using Microsoft.AspNetCore.Http.HttpResults;
using Documind.Application;

namespace Documind.Endpoints;

public static class SeedEndpoints
{
    public static void MapSeedEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/seed")
                       .WithTags("Seed");

        // We inject IngestionService directly into the lambda
        group.MapPost("/",
            async Task<Results<BadRequest<string>, Ok<string>>>
                (KnowledgeSeeder service) =>
        {
            await service.SeedAsync();
            return TypedResults.Ok("Seed executed successfully.");
        })
        .WithName("SeedData");
    }
}