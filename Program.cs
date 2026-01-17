using System.Text.Json.Serialization;
using Documind.Adapters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

// This is the WebApplicationBuilder - it has the .Configuration property
var builder = WebApplication.CreateSlimBuilder(args);

// 1. Get Key
string geminiKey = builder.Configuration["Gemini:ApiKey"]
                   ?? throw new Exception("Gemini key not defined");

// 2. Register Kernel Services
var kernelBuilder = builder.Services.AddKernel();

// REGISTER CHAT (For the "/ask" part later)
kernelBuilder.AddGoogleAIGeminiChatCompletion(
    modelId: "gemini-1.5-flash",
    apiKey: geminiKey);

// REGISTER EMBEDDINGS (Required for "IngestAsync")
kernelBuilder.AddGoogleAIEmbeddingGenerator(
    modelId: "text-embedding-004", // Use the specialized embedding model
    apiKey: geminiKey);

// 3. Register Postgres Vector Store
string connString = "Host=localhost;Port=5432;Database=documind_db;Username=postgres;Password=secret1";
builder.Services.AddPostgresVectorStore(connString);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IngestionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
