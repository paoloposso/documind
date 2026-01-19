using System.Text.Json.Serialization;
using Documind.Adapters;
using Documind.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.AspNetCore.Routing.Constraints;
using Documind.Domain;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.Configure<RouteOptions>(options =>
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

string geminiKey = builder.Configuration["Gemini:ApiKey"]
                   ?? throw new Exception("Gemini key not defined");

var kernelBuilder = builder.Services.AddKernel();

builder.Services.AddGoogleAIGeminiChatCompletion(modelId: "gemini-1.5-flash", apiKey: geminiKey);
builder.Services.AddGoogleAIEmbeddingGenerator(modelId: "text-embedding-004", apiKey: geminiKey);

// kernelBuilder.AddGoogleAIGeminiChatCompletion(
//     modelId: "gemini-1.5-flash",
//     apiKey: geminiKey);

// kernelBuilder.AddGoogleAIEmbeddingGenerator(
//     modelId: "text-embedding-004", // Use the specialized embedding model
//     apiKey: geminiKey);

string connString = builder.Configuration.GetConnectionString("Postgres")
                    ?? throw new Exception("Postgres connection string not defined");

builder.Services.AddPostgresVectorStore(connString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IngestionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIngestionEndpoints();

app.Run();

[JsonSerializable(typeof(DocumentRecord))]
[JsonSerializable(typeof(List<DocumentRecord>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(Guid))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}