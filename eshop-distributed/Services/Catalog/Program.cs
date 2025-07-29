using Catalog.Data;
using Catalog.Endpoints;
using Catalog.Services;
using ServiceDefaults.Messages;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogDb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductAIService>();

builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

builder.AddOllamaApiClient("ollama-llama3-2-chat")
       .AddChatClient();
builder.AddOllamaApiClient("ollama-all-minilm")
       .AddEmbeddingGenerator();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMigration();

app.MapProductEndpoints();

app.UseHttpsRedirection();

app.Run();