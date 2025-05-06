using Basket.ApiClients;
using Basket.Endpoints;
using Basket.Services;
using ServiceDefaults.Messages;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();
builder.Services.AddScoped<BasketService>();
builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new("http+https://catalog");
});
builder.Services.AddMassTransitWithAssemblies(Assembly.GetExecutingAssembly());

builder.AddRedisDistributedCache(connectionName: "redis");

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();
app.MapBasketEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();