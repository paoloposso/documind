using System.Text.Json.Serialization;
using Documind.Application;
using Documind.Endpoints;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.Routing.Constraints;
using Documind.Domain;
using Documind.Infrastructure;
using Documind.Application.Abstractions;
using Documind.Application.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.Configure<RouteOptions>(options =>
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

string geminiKey = builder.Configuration["Gemini:ApiKey"]
                   ?? throw new Exception("Gemini key not defined");

var kernelBuilder = builder.Services.AddKernel();

builder.Services.AddGoogleAIGeminiChatCompletion(modelId: "gemini-2.5-flash", apiKey: geminiKey);
builder.Services.AddGoogleAIEmbeddingGenerator(modelId: "gemini-embedding-001", apiKey: geminiKey);

string connString = builder.Configuration.GetConnectionString("Postgres")
                    ?? throw new Exception("Postgres connection string not defined");

builder.Services.AddPostgresVectorStore(connString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDocumentRepository, VectorStoreDocumentRepository>();
builder.Services.AddScoped<IIngestionService, IngestionService>();
builder.Services.AddScoped<IKnowledgeSeeder, KnowledgeSeeder>();
builder.Services.AddScoped<IAskService, AskService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IFileIngestionService, FileIngestionService>();

string redisConnString = builder.Configuration.GetConnectionString("Redis")
                         ?? "localhost:6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnString;
});

builder.Services.AddSingleton<IIngestionJobQueue, IngestionJobQueue>();
builder.Services.AddScoped<IJobStatusService, RedisJobStatusService>();
builder.Services.AddHostedService<IngestionBackgroundService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIngestionEndpoints();
app.MapSeedEndpoints();
app.MapSearchEndpoints();
app.MapAskEndpoints();
app.MapFileIngestionEndpoints();

app.Run();

[JsonSerializable(typeof(DocumentRecord))]
[JsonSerializable(typeof(List<DocumentRecord>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(Guid))]
[JsonSerializable(typeof(JobStatusResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}